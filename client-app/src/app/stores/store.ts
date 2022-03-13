import ActivityStore from "./activityStore";
import { createContext, useContext } from "react";
import CommonStore from "./commonStore";
import UserStore from "./userStore";
import ModalStore from "./modalStore";

interface Store {
  //ActivityStore is a class, but class can be also used as a type
  activityStore: ActivityStore;
  commonStore: CommonStore;
  userStore: UserStore;
  modalStore: ModalStore;
}

export const store: Store = {
  activityStore: new ActivityStore(),
  commonStore: new CommonStore(),
  userStore: new UserStore(),
  modalStore: new ModalStore(),
};

// Create store is coming from React
export const StoreContext = createContext(store);

// React hook that will allow us to use our stores inside our components (useContext is also React hook)
export function useStore() {
  return useContext(StoreContext);
}
