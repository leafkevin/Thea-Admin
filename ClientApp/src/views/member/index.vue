<template>
  <div class="table-box">
    <ProTable
      ref="tableRef"
      title="会员列表"
      row-key="memberId"
      :indent="20"
      :columns="columns"
      :request-api="queryPage"
      :search-column-widths="{ xs: 1, sm: 1, md: 2, lg: 3, xl: 3 }"
      highlight-current-row>
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <el-button type="primary" :icon="CirclePlus" @click="router.push({ name: 'MemberEdit' })">新增会员</el-button>
        <el-button type="primary" :icon="Upload" plain @click="batchAdd">批量添加</el-button>
        <el-button type="primary" :icon="Download" plain @click="downloadFile">导出</el-button>
        <el-button type="danger" :icon="Delete" plain :disabled="!scope.isSelected" @click="batchDelete(scope.selectedListIds)">
          批量删除
        </el-button>
      </template>
      <template #gender="scope">
        <el-tag type="info" v-if="scope.row.gender == 0">未知</el-tag>
        <el-tag type="danger" v-else-if="scope.row.gender == 2">女性</el-tag>
        <el-tag type="" v-else>男性</el-tag>
      </template>

      <template #balance="scope">
        {{ new Intl.NumberFormat("zh-CN", { style: "currency", currency: "CNY" }).format(scope.row.balance) }}
      </template>
      <!-- 菜单操作 -->
      <template #operation>
        <el-button type="primary" link :icon="EditPen"> 编辑 </el-button>
        <el-button type="primary" link :icon="Delete" @click="deleteMember"> 删除 </el-button>
      </template>
    </ProTable>
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";
  import { IColumnProps, ProTableInstance } from "@/components/ProTable/interface";
  import { Delete, EditPen, CirclePlus, Upload, Download } from "@element-plus/icons-vue";
  import ProTable from "@/components/ProTable/index.vue";
  import { useRouter } from "vue-router";
  import { useHandleData } from "@/hooks/useHandleData";
  import { IMemberSate, queryPage, deleteMember as deleteMemberApi, batchDeleteMembers, exportMembers } from "@/api/member";
  import { ElMessageBox } from "element-plus";
  import { useDownload } from "@/hooks/useDownload";

  defineOptions({
    name: "MemberList"
  });
  const router = useRouter();
  const tableRef = ref<ProTableInstance>();
  // 表格配置项
  const columns: IColumnProps[] = [
    { type: "selection", fixed: "left", width: 70 },
    { prop: "memberId", label: "会员ID", align: "left" },
    { prop: "memberName", label: "会员名称", align: "center", search: { el: "input", props: { placeholder: "请输入会员名称" } } },
    { prop: "mobile", label: "手机号码", align: "center", search: { el: "input", props: { placeholder: "请输入手机号码" } } },
    { prop: "gender", label: "性别", align: "center" },
    { prop: "balance", label: "余额", align: "right" },
    { prop: "operation", label: "操作", width: 250, fixed: "right" }
  ];

  // 删除会员信息
  const deleteMember = async (params: IMemberSate) => {
    await useHandleData(deleteMemberApi, { id: [params.memberId] }, `删除【${params.memberName}】用户`);
    tableRef.value?.getTableList();
  };
  // 批量删除用户信息
  const batchDelete = async (ids: string[]) => {
    await useHandleData(batchDeleteMembers, { ids }, "删除所选会员信息");
    tableRef.value?.clearSelection();
    tableRef.value?.getTableList();
  };
  // 导出用户列表
  const downloadFile = async () => {
    ElMessageBox.confirm("确认导出用户数据?", "温馨提示", { type: "warning" }).then(() =>
      useDownload(exportMembers, "用户列表", tableRef.value?.searchParameters)
    );
  };
  // 批量添加用户
  // const dialogRef = ref<InstanceType<typeof ImportExcel> | null>(null);
  const batchAdd = () => {
    // const params = {
    //   title: "用户",
    //   tempApi: exportUserInfo,
    //   importApi: BatchAddUser,
    //   getTableList: tableRef.value?.getTableList
    // };
    // dialogRef.value?.acceptParams(params);
  };
</script>
