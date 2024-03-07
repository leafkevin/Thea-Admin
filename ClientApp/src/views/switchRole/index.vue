<template>
  <div class="main-box">
    <ProTable ref="proTable" title="我的角色列表" highlight-current-row :columns="columns" :request-api="queryList">
      <!-- 表格 header 按钮 -->
      <template #tableHeader>
        <el-text class="mx-1">我的角色列表</el-text>
      </template>
      <!-- 表格操作 -->
      <template #operation="scope">
        <el-button type="primary" link :icon="Check" @click="switchRole(scope.row)"> 查看</el-button>
      </template>
    </ProTable>
  </div>
</template>
<script setup lang="ts">
  import { ref, reactive, onMounted } from "vue";
  import { IRoleState } from "./types";
  import { ColumnProps } from "@/components/ProTable/interface/index";
  import { getMyRoles as getMyRolesApi, switchRole as switchRoleApi } from "@/api/account";
  import { ElNotification } from "element-plus";
  import { Check } from "@element-plus/icons-vue";
  const dataList = ref<IRoleState[]>([]);

  // 表格配置项
  const columns = reactive<ColumnProps<IRoleState>[]>([
    { prop: "roleId", label: "角色ID", width: 120 },
    { prop: "roleName", label: "角色名称", width: 120 },
    { prop: "description", label: "描述" },
    { prop: "operation", label: "选择", width: 330 }
  ]);

  const switchRole = async (row: IRoleState) => {
    const resp = await switchRoleApi({ roleId: row.roleId });
    const response = resp.data;
    if (!response.isSuccess) {
      ElNotification({
        title: "切换角色失败",
        message: `切换角色失败，${response.message}，code:${response.code}`,
        type: "error"
      });
    }
    await queryList();
  };
  const queryList = async () => {
    const resp = await getMyRolesApi();
    const response = resp.data;
    if (!response.isSuccess) {
      ElNotification({
        title: "获取角色失败",
        message: `获取角色失败，${response.message}，code:${response.code}`,
        type: "error"
      });
    }
    dataList.value = response.data as IRoleState[];
  };
  onMounted(async () => {
    await queryList();
  });
</script>

<style></style>
