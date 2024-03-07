import { http } from "./http";

export interface ILoginRequest {
  userAccount: string;
  password: string;
}
export const login = (parameters: { userAccount: string; password: string }) => {
  return http.post("/account/login", parameters);
};
export const refreshToken = (parameters: { refreshToken: string | undefined }) => {
  return http.post("/account/refreshToken", parameters);
};
export const resetPassword = (parameters: { password: string }) => {
  return http.post("/profile/resetPassword", parameters);
};
export const getMyRoles = () => {
  return http.get("/profile/getMyRoles", {});
};
export const switchRole = (parameters: { roleId: string }) => {
  return http.post("/profile/switchRole", parameters);
};
