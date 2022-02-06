import ActivityStore from "./activityStore";
import { createContext, useContext } from "react";

interface Store {
  //ActivityStore is a class, but class can be also used as a type
  activityStore: ActivityStore;
}

export const store: Store = {
  activityStore: new ActivityStore(),
};

// Create store is coming from React
export const StoreContext = createContext(store);

// React hook that will allow us to use our stores inside our components (useContext is also React hook)
export function useStore() {
  return useContext(StoreContext);
}
