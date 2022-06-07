import { makeAutoObservable, reaction, runInAction } from "mobx";
import agent from "../api/agent";
import { ActivityFormValues, IActivity } from "../models/activity";
import { format } from "date-fns";
import { store } from "./store";
import { IProfile } from "../models/profile";
import { IPagination, PagingParams } from "../models/pagination";

export default class ActivityStore {
  //Map with 2 types => first one is key which represents activity id and the second will be Activity object iteself
  activityRegistry = new Map<string, IActivity>();
  selectedActivity: IActivity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;
  pagination: IPagination | null = null;
  pagingParams = new PagingParams();
  predicate = new Map().set("all", true);

  constructor() {
    makeAutoObservable(this);

    //we are adding reaction and we want to observe if some keys in predicate changes and then we want ot load activities
    reaction(
      () => this.predicate.keys(),
      () => {
        this.pagingParams = new PagingParams();
        this.activityRegistry.clear();
        this.loadActivities();
      }
    );
  }

  setPagingParams = (pagingParams: PagingParams) => {
    this.pagingParams = pagingParams;
  };

  setPredicate = (predicate: string, value: string | Date) => {
    //we want to reset our predicates in case we pick up different filers in our UI
    const resetPredicate = () => {
      this.predicate.forEach((value, key) => {
        if (key !== "startDate") this.predicate.delete(key);
      });
    };

    switch (predicate) {
      case "all":
        resetPredicate();
        this.predicate.set("all", true);
        break;
      case "isGoing":
        resetPredicate();
        this.predicate.set("isGoing", true);
        break;
      case "isHost":
        resetPredicate();
        this.predicate.set("isHost", true);
        break;
      case "startDate":
        this.predicate.delete("startDate");
        this.predicate.set("startDate", value);
    }
  };

  get axiosParams() {
    const params = new URLSearchParams();
    params.append("pageNumber", this.pagingParams.pageNumber.toString());
    params.append("pageSize", this.pagingParams.pageSize.toString());
    this.predicate.forEach((value, key) => {
      if (key === "startDate") {
        params.append(key, (value as Date).toISOString());
      } else {
        params.append(key, value);
      }
    });
    return params;
  }

  get activitiesByDate() {
    return Array.from(this.activityRegistry.values()).sort(
      //we have to used exclamation mark here, because we have defined that date can be type Date or null, because in activity form as initial state we want to use null so we don't have any date spcified
      (a, b) => a.date!.getTime() - b.date!.getTime()
    );
  }

  get groupedActivities() {
    return Object.entries(
      this.activitiesByDate.reduce((activities, activity) => {
        //this will represent key for each of our objects
        const date = format(activity.date!, "dd MMM yyyy");
        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity];
        return activities;
        // initial value is empty object as type [key: string]: IActivity[]
      }, {} as { [key: string]: IActivity[] })
    );
  }

  loadActivities = async () => {
    //any code that is asynchronious should go outside of try catch statement
    this.loadingInitial = true;
    try {
      const result = await agent.Activities.list(this.axiosParams);

      result.data.forEach((activity) => {
        this.setActivity(activity);
      });
      this.setPagination(result.pagination);
      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);

      this.setLoadingInitial(false);
    }
  };

  loadActivity = async (id: string) => {
    let activity = this.getActivity(id);
    if (activity) {
      this.selectedActivity = activity;
      //we are returning here activity so we can use it in Activity Form, where we are direclty calling loadActivity method in useEffect with then and set local state. If we use selectedActivity in  ActivityForm after useEffect with loadActivity(id) we will have timing issue. Our initial state will be empty, then we will loadAcitivty, which will change selected activity and then another render for component. With current solution we will have less rendering
      return activity;
      //if there is no activity loaded in our store, then we have to make api call
    } else {
      this.loadingInitial = true;
      try {
        activity = await agent.Activities.details(id);
        this.setActivity(activity);
        runInAction(() => {
          this.selectedActivity = activity;
        });
        this.setLoadingInitial(false);
        return activity;
      } catch (error) {
        console.log(error);
        this.setLoadingInitial(false);
      }
    }
  };

  setPagination = (pagination: IPagination) => {
    this.pagination = pagination;
  };

  private setActivity = (activity: IActivity) => {
    //  We are getting information of cureently logged user
    const user = store.userStore.user;

    if (user) {
      activity.isGoing = activity.attendees!.some(
        (a) => a.userName === user.userName
      );
      activity.isHost = activity.hostUsername === user.userName;
      activity.host = activity.attendees?.find(
        (x) => x.userName === activity.hostUsername
      );
    }
    //old code when we were using date as string
    // activity.date = activity.date.split("T")[0];
    activity.date = new Date(activity.date!);
    this.activityRegistry.set(activity.id, activity);
  };

  private getActivity = (id: string) => {
    return this.activityRegistry.get(id);
  };

  //error with async await and mobx => we have to put code used after async in runInAction handlerer
  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };

  createActivity = async (activity: ActivityFormValues) => {
    const user = store.userStore.user;
    const attendee = new IProfile(user!);
    try {
      await agent.Activities.create(activity);
      const newActivity = new IActivity(activity);
      newActivity.hostUsername = user!.userName;
      newActivity.attendees = [attendee];
      this.setActivity(newActivity);
      //this is because of mobx and async await functions => if we run code that changes the state in try await block then have to use this.
      runInAction(() => {
        this.selectedActivity = newActivity;
      });
    } catch (error) {
      console.log(error);
    }
  };

  updateActivity = async (activity: ActivityFormValues) => {
    this.loading = true;
    try {
      await agent.Activities.update(activity);
      runInAction(() => {
        // Before changing code to MAP: here we are using spread operator and not modifing state => basically we are coping state without old activity and then replacing it with updated one, id is not being updated
        // this.activities = [
        //   ...this.activities.filter((a) => a.id !== activity.id),
        //   activity,
        // ];
        if (activity.id) {
          let updatedActivity = {
            ...this.getActivity(activity.id),
            ...activity,
          };
          this.activityRegistry.set(activity.id, updatedActivity as IActivity);
          this.selectedActivity = updatedActivity as IActivity;
        }
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  deleteActivity = async (id: string) => {
    this.loading = true;
    try {
      await agent.Activities.delete(id);
      runInAction(() => {
        this.activityRegistry.delete(id);
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  updateAttendance = async () => {
    const user = store.userStore.user;
    this.loading = true;

    try {
      // 1st updating attendance on the backend
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        //2nd Updating attendace in store. This is working as a toggle => we check with if selectedActivity isGoing and then changed it to not going
        if (this.selectedActivity?.isGoing) {
          // We are going to filter out currently logged in user
          this.selectedActivity.attendees =
            this.selectedActivity.attendees?.filter(
              (a) => a.userName !== user?.userName
            );
          this.selectedActivity.isGoing = false;
        } else {
          //3rd here we have to create new profile attendee
          const attendee = new IProfile(user!);
          this.selectedActivity?.attendees?.push(attendee);
          this.selectedActivity!.isGoing = true;
        }
        //4th which ever option is correct => we have to update activityRegistry
        this.activityRegistry.set(
          this.selectedActivity!.id,
          this.selectedActivity!
        );
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction(() => (this.loading = false));
    }
  };

  cancelActivityToggle = async () => {
    this.loading = true;
    try {
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        this.selectedActivity!.isCancelled =
          !this.selectedActivity?.isCancelled;
        this.activityRegistry.set(
          this.selectedActivity!.id,
          this.selectedActivity!
        );
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction(() => (this.loading = false));
    }
  };

  clearSelectedActivity = () => {
    this.selectedActivity = undefined;
  };

  updateAttendeeFollowing = (username: string) => {
    this.activityRegistry.forEach((activity) => {
      activity.attendees.forEach((attendee) => {
        if (attendee.userName === username) {
          attendee.following
            ? attendee.followersCount--
            : attendee.followersCount++;
          attendee.following = !attendee.following;
        }
      });
    });
  };
}
