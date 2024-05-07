import axios from "axios";
import { getAuth } from "../modules/auth";

const API_URL = import.meta.env.VITE_APP_API_URL;
export function post<T>(url: string, data: any) {
    var auth = getAuth();
    var headers = {
        "Authorization": "Bearer " + auth?.token
    };
    var response = axios.post<T>(API_URL + url, data, {
        headers: headers
    });
    return response;
}

export function get<T>(url: string) {
    var auth = getAuth();
    var headers = {
        "Authorization": "Bearer " + auth?.token
    };
    var response = axios.get<T>(API_URL + url, {
        headers: headers
    });
    return response;
}

export function deleteRequest<T>(url: string) {
    var auth = getAuth();
    var headers = {
        "Authorization": "Bearer " + auth?.token
    };
    var response = axios.delete<T>(API_URL + url, {
        headers: headers
    });
    return response;
}