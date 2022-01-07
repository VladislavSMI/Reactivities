import ActivityStore from "./activityStore";
import { createContext, useContext } from "react";

interface Store {
  //ActivityStore is a class, but class can be also used as a type
  activityStore: ActivityStore;
}

export const store: Store = {
  activityStore: new ActivityStore(),
};

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}
