import axios from "axios";
import { AuthModel, UserModel } from "./_models";
import { get, post } from "../../../api/make-api-request-authorized";
import ServiceResponse, { ServiceResponseWithoutData } from "../../../../models/service-response";

const API_URL = import.meta.env.VITE_APP_API_URL;

export const LOGIN_URL = `/account/login`;
export const REGISTER_URL = `/account/register`;
export const REQUEST_PASSWORD_URL = `/account/forgot-password`;
export const GET_CLAIMS_REQUEST_URL = `/account/get-claims`;

// Server should return AuthModel
export function login(email: string, password: string) {
  return post<ServiceResponse<AuthModel>>(LOGIN_URL, {
    email,
    password,
  });
}

// Server should return AuthModel
export function register(
  email: string,
  password: string,
  password_confirmation: string
) {
  return post<any>(REGISTER_URL, {
    email,
    password,
    passwordConfirmation: password_confirmation,
  });
}

// Server should return object => { result: boolean } (Is Email in DB)
export function requestPassword(email: string) {
  return post<ServiceResponse<boolean>>(REQUEST_PASSWORD_URL, {
    email,
  });
}

export function getClaims() {
  return get<ServiceResponse<UserModel>>(GET_CLAIMS_REQUEST_URL);
}
