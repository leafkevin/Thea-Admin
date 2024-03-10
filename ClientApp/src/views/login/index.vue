<template>
  <div class="login-container flx-center">
    <div class="login-box">
      <SwitchDark class="dark" />
      <div class="login-left">
        <img class="login-left-img" src="@/assets/images/login_left.png" alt="login" />
      </div>
      <div class="login-form">
        <div class="login-logo">
          <img class="login-icon" src="@/assets/images/logo.svg" alt="" />
          <h2 class="logo-text">Thea</h2>
        </div>
        <el-form ref="loginFormRef" :model="loginForm" :rules="rules" size="large">
          <el-form-item prop="userAccount">
            <el-input v-model="loginForm.userAccount" placeholder="用户名：admin">
              <template #prefix>
                <el-icon class="el-input__icon">
                  <user />
                </el-icon>
              </template>
            </el-input>
          </el-form-item>
          <el-form-item prop="password">
            <el-input
              v-model="loginForm.password"
              type="password"
              placeholder="密码：admin123"
              show-password
              autocomplete="new-password">
              <template #prefix>
                <el-icon class="el-input__icon">
                  <lock />
                </el-icon>
              </template>
            </el-input>
          </el-form-item>
        </el-form>
        <div class="login-btn">
          <el-button :icon="CircleClose" round size="large" @click="resetForm(loginFormRef)"> 重置 </el-button>
          <el-button :icon="UserFilled" round size="large" type="primary" :loading="loading" @click="login(loginFormRef)">
            登录
          </el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts" name="login">
  import { ref, onMounted } from "vue";
  import { useRouter } from "vue-router";
  import { getTimeState } from "@/utils";
  import { ElNotification } from "element-plus";
  import { ILoginResponse, login as loginApi } from "@/api/account";
  import { useUserStore } from "@/stores/account";
  import { useTabPageStore } from "@/stores/tabPages";
  import { useKeepAliveStore } from "@/stores/keepAlive";
  import { CircleClose, UserFilled } from "@element-plus/icons-vue";
  import type { ElForm } from "element-plus";
  import SwitchDark from "@/components/SwitchDark/index.vue";
  import { HOME_URL } from "@/config";
  import { useMenuStore } from "@/stores/menu";

  const router = useRouter();
  const userStore = useUserStore();
  const menuStore = useMenuStore();

  defineOptions({
    name: "Login"
  });

  type FormInstance = InstanceType<typeof ElForm>;
  const loginFormRef = ref<FormInstance>();
  const rules = {
    userAccount: [{ required: true, message: "请输入用户名", trigger: "blur" }],
    password: [{ required: true, message: "请输入密码", trigger: "blur" }]
  };

  const loading = ref(false);
  const loginForm = ref<{ userAccount: string; password: string }>({
    userAccount: "leafkevin",
    password: "admin123"
  });

  // login
  const login = async (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    formEl.validate(async valid => {
      if (!valid) return;
      loading.value = true;
      try {
        // 1.执行登录接口
        const response = await loginApi(loginForm.value);
        if (!response.isSuccess) {
          ElNotification({
            title: "登录失败",
            message: `登录失败，${response.message}，code:${response.code}`,
            type: "error"
          });
        }
        const loginResponse = response.data as ILoginResponse;
        userStore.setState(loginResponse);
        menuStore.setState(loginResponse.menuRoutes);

        //需要转到选择角色页面 code=9
        if (response.code > 0) {
          router.push({ name: "SwitchRole", replace: true });
          return;
        }
        //添加动态路由
        if (menuStore.hasMenuRoutes) {
          menuStore.loadAsyncMenuRoutes();

          const keepAliveStore = useKeepAliveStore();
          const tabPageStore = useTabPageStore();
          keepAliveStore.setKeepAliveNames([]);
          tabPageStore.setTabPages([]);
        }

        // 4.跳转到首页
        router.push(HOME_URL);
        ElNotification({
          title: getTimeState(),
          message: `登录成功，欢迎${userStore.userName}`,
          type: "success",
          duration: 3000
        });
      } finally {
        loading.value = false;
      }
    });
  };

  // resetForm
  const resetForm = (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    formEl.resetFields();
  };

  onMounted(() => {
    // 监听 enter 事件（调用登录）
    document.onkeydown = (e: KeyboardEvent) => {
      e = (window.event as KeyboardEvent) || e;
      if (e.code === "Enter" || e.code === "enter" || e.code === "NumpadEnter") {
        if (loading.value) return;
        login(loginFormRef.value);
      }
    };
  });
</script>

<style scoped lang="scss">
  @import "./index.scss";
</style>
