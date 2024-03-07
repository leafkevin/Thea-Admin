import { ref } from "vue";
import { defineStore } from "pinia";

export const useKeepAliveStore = defineStore(
  "keepAlive",
  () => {
    const keepAliveNames = ref<string[]>([]);
    function addKeepAliveName(name: string) {
      !keepAliveNames.value.includes(name) && keepAliveNames.value.push(name);
    }
    // Remove KeepAliveName
    function removeKeepAliveName(name: string) {
      keepAliveNames.value = keepAliveNames.value.filter(item => item !== name);
    }
    // Set KeepAliveName
    function setKeepAliveNames(names: string[] = []) {
      keepAliveNames.value = names;
    }
    return { keepAliveNames, addKeepAliveName, removeKeepAliveName, setKeepAliveNames };
  },
  {
    persist: true
  }
);
