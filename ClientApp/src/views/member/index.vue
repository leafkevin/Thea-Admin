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
        <el-button type="primary" :icon="CirclePlus" @click="createMember">新增会员</el-button>
        <el-button type="primary" :icon="Upload" plain @click="batchImport">批量导入</el-button>
        <el-button type="primary" :icon="Download" plain @click="downloadFile">导出</el-button>
        <el-button type="danger" :icon="Delete" plain :disabled="!scope.isSelected" @click="batchDelete(scope.selectedListIds)">
          批量删除
        </el-button>
      </template>
      <template #gender="scope">
        <el-tag type="info" v-if="scope.row.gender == 0">未知</el-tag>
        <el-tag type="primary" v-else-if="scope.row.gender == 1">男性</el-tag>
        <el-tag type="danger" v-else>女性</el-tag>
      </template>

      <template #balance="scope">
        {{ new Intl.NumberFormat("zh-CN", { style: "currency", currency: "CNY" }).format(scope.row.balance) }}
      </template>
      <!-- 菜单操作 -->
      <template #operation="scope">
        <el-button type="primary" link :icon="EditPen" @click="modifyMember(scope)"> 编辑 </el-button>
        <el-button type="danger" link :icon="Delete" @click="deleteMember(scope)"> 删除 </el-button>
      </template>
    </ProTable>
    <ImportExcel ref="dialogRef" />
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";
  import { IColumnProps, ProTableInstance } from "@/components/ProTable/interface";
  import { Delete, EditPen, CirclePlus, Upload, Download } from "@element-plus/icons-vue";
  import ProTable from "@/components/ProTable/index.vue";
  import { useRouter } from "vue-router";
  import { useHandleData } from "@/hooks/useHandleData";
  import {
    queryPage,
    importMembers,
    deleteMember as deleteMemberApi,
    batchDeleteMembers,
    exportMembers,
    downloadTemplate
  } from "@/api/member";
  import { ElMessageBox } from "element-plus";
  import { useDownload } from "@/hooks/useDownload";
  import ImportExcel from "@/components/ImportExcel/index.vue";

  defineOptions({
    name: "MemberList"
  });

  const router = useRouter();
  const tableRef = ref<ProTableInstance>();
  // 表格配置项
  const columns: IColumnProps[] = [
    { type: "selection", fixed: "left", width: 70 },
    { prop: "memberName", label: "会员名称", align: "center", search: { el: "input", props: { placeholder: "请输入会员名称" } } },
    { prop: "mobile", label: "手机号码", align: "center", search: { el: "input", props: { placeholder: "请输入手机号码" } } },
    { prop: "gender", label: "性别", align: "center" },
    { prop: "balance", label: "余额", align: "right" },
    { prop: "description", label: "备注", align: "left" },
    { prop: "createdAt", label: "注册时间", minWidth: 100 },
    { prop: "operation", label: "操作", minWidth: 120, fixed: "right" }
  ];
  // 新增会员信息
  const createMember = () => {
    router.push({ name: "MemberEdit", state: { id: "" } });
  };
  // 批量添加用户
  const dialogRef = ref<InstanceType<typeof ImportExcel> | null>(null);
  const batchImport = () => {
    const params = {
      title: "会员批量导入",
      templateName: "会员导入模板",
      skipContent: "存在相同手机号码的会员数据，将被跳过不会导入！",
      tempApi: downloadTemplate,
      importApi: importMembers,
      getTableList: tableRef.value?.getTableList
    };
    dialogRef.value?.acceptParameters(params);
  };

  const modifyMember = scope => router.push({ name: "MemberEdit", state: { id: scope.row.memberId } });
  // 删除会员信息
  const deleteMember = async scope => {
    await useHandleData(deleteMemberApi, { id: scope.row.memberId }, `删除【${scope.row.memberName}】用户`);
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
      useDownload(exportMembers, "会员列表导出", tableRef.value?.searchParameters)
    );
  };
</script>
