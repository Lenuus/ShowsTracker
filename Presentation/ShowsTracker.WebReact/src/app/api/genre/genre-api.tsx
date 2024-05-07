import axios from "axios";
import { GetAllGenresRequestModel } from "../../../models/genre/get-all-genres-request-model";
import ServiceResponse from "../../../models/service-response";
import { PagedResponseModel } from "../../../models/paged-response-model";
import { GetAllGenresResponseModel } from "../../../models/genre/get-all-genres-response-model";

const API_URL = import.meta.env.VITE_APP_API_URL;
export function getAllGenres(request: GetAllGenresRequestModel) {
    var response = axios.post<ServiceResponse<PagedResponseModel<GetAllGenresResponseModel>>>(API_URL + "/genres/get-genres", request);
    return response;
}