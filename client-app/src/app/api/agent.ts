import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { history } from "../../index";
import { IActivity } from "../models/activity";
import { store } from "../stores/store";

//function to mimic delays in response from API in real environment
const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

axios.defaults.baseURL = "http://localhost:5000/api";

axios.interceptors.response.use(
  async (response) => {
    await sleep(1000);
    return response;
  },
  (error: AxiosError) => {
    const { data, status, config } = error.response!;
    switch (status) {
      case 400:
        // We have 2 types of 400 reponses(Bad Request & Validation Error) If data contains error object then it is Validation Error and not Bad Request error
        const { errors } = data;
        const { method } = config;

        if (typeof data === "string") {
          toast.error(data);
        }

        if (method === "get" && errors.hasOwnProperty("id")) {
          history.push("/not-found");
        }
        if (errors) {
          const modalStateErrors = [];
          for (const key in errors) {
            if (errors[key]) {
              modalStateErrors.push(errors[key]);
            }
          }
          throw modalStateErrors.flat();
        }
        break;
      case 401:
        toast.error("unauthorised");
        break;
      case 404:
        history.push("/not-found");
        break;
      case 500:
        store.commonStore.setServerError(data);
        history.push("/server-error");
        break;
    }
    return Promise.reject(error);
  }
);

// we have to add generic type for type safty
const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) =>
    axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Activities = {
  list: () => requests.get<IActivity[]>("/activities"),
  details: (id: string) => requests.get<IActivity>(`/activities/${id}`),
  create: (activity: IActivity) => requests.post<void>("/activities", activity),
  update: (activity: IActivity) =>
    requests.put<void>(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.del<void>(`/activities/${id}`),
};

const agent = {
  Activities,
};

export default agent;
