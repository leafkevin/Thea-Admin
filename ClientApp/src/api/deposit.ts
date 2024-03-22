import { http } from "./http";

export interface IDepositState {
  memberId: string;
  memberName: string;
  mobile: string;
  balance: number;
  depositTimes: Date;
  lastDepositedAt: Date;
}
export const queryPage = (parameters: object) => {
  return http.post("/deposit/queryPage", parameters);
};
export const createMember = (parameters: object) => {
  return http.post("/deposit/create", parameters);
};
export const exportDeposits = (parameters: object) => {
  return http.download("/deposit/export", parameters);
};
