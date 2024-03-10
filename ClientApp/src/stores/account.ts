import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { IUserState } from "./types";

export const useUserStore = defineStore(
  "user",
  () => {
    const userId = ref<string>();
    const userName = ref<string>();
    const accessToken = ref<string>();
    const refreshToken = ref<string>();
    const roles = ref<string>();
    const expiresTime = ref<number>(0);

    const isExpired = computed(() => expiresTime.value <= new Date().getTime() / 1000);
    const isAuthorized = computed(() => userId.value !== undefined);
    const isMultiRoles = computed(() => roles.value?.includes(","));

    function setState(state: IUserState) {
      userId.value = state.userId;
      userName.value = state.userName;
      accessToken.value = state.accessToken;
      refreshToken.value = state.refreshToken;
      roles.value = state.roles;
      expiresTime.value = state.expires;
    }
    function clearState() {
      userId.value = undefined;
      userName.value = undefined;
      accessToken.value = undefined;
      refreshToken.value = undefined;
      roles.value = undefined;
    }
    return {
      userId,
      userName,
      accessToken,
      refreshToken,
      roles,
      expiresTime,
      isAuthorized,
      isMultiRoles,
      isExpired,
      setState,
      clearState
    };
  },
  {
    persist: true
  }
);
