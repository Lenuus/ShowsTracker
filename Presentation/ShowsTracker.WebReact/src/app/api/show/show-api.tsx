import axios from "axios";
import { ShowListRequestModel } from "../../../models/show/show-list-request-model";
import { ShowListModel } from "../../../models/show/show-list-model";
import ServiceResponse from "../../../models/service-response";
import { PagedResponseModel } from "../../../models/paged-response-model";

const API_URL = import.meta.env.VITE_APP_API_URL;
export function getAllShows(request: ShowListRequestModel) {
    var response = axios.post<ServiceResponse<PagedResponseModel<ShowListModel>>>(API_URL + "/show/get-shows", request);
    return response;
}