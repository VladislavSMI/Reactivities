import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { IActivity } from "../models/activity";
import { format } from "date-fns";

export default class ActivityStore {
  //Map with 2 types => first one is key which represents activity id and the second will be Activity object iteself
  activityRegistry = new Map<string, IActivity>();
  selectedActivity: IActivity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this);
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
      const activities = await agent.Activities.list();

      activities.forEach((activity) => {
        this.setActivity(activity);
      });
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

  private setActivity = (activity: IActivity) => {
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

  createActivity = async (activity: IActivity) => {
    this.loading = true;

    try {
      await agent.Activities.create(activity);
      //this is because of mobx and async await functions => if we run code that changes the state in try await block then have to use this.
      runInAction(() => {
        this.activityRegistry.set(activity.id, activity);
        this.selectedActivity = activity;
        this.editMode = false;
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  updateActivity = async (activity: IActivity) => {
    this.loading = true;
    try {
      await agent.Activities.update(activity);
      runInAction(() => {
        // Before changing code to MAP: here we are using spread operator and not modifing state => basically we are coping state without old activity and then replacing it with updated one, id is not being updated
        // this.activities = [
        //   ...this.activities.filter((a) => a.id !== activity.id),
        //   activity,
        // ];

        this.activityRegistry.set(activity.id, activity);
        this.editMode = false;
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
}
