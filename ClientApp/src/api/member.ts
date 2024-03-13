import { http } from "./http";

export interface IMemberState {
  memberId?: string;
  memberName: string;
  mobile: string;
  gender: number;
  balance: number;
  description: string;
}
export const queryPage = (parameters: object) => {
  return http.post("/member/queryPage", parameters);
};
export const createMember = (parameters: object) => {
  return http.post("/member/create", parameters);
};
export const modifyMember = (parameters: object) => {
  return http.post("/member/modify", parameters);
};
export const deleteMember = (parameters: object) => {
  return http.post("/member/delete", parameters);
};
export const batchDeleteMembers = (parameters: object) => {
  return http.get("/member/batchDelete", parameters);
};
export const exportMembers = (parameters: object) => {
  return http.get("/member/export", parameters);
};
