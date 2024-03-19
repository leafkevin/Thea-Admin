<template>
  <div class="table-box">
    <ProTable
      ref="tableRef"
      title="会员充值列表"
      row-key="memberId"
      :indent="20"
      :columns="columns"
      :request-api="queryPage"
      :search-column-widths="{ xs: 1, sm: 1, md: 2, lg: 3, xl: 3 }"
      highlight-current-row>
      <!-- 表格 header 按钮 -->
      <template #tableHeader>
        <el-button type="primary" :icon="Download" plain @click="downloadFile">导出</el-button>
      </template>
      <template #balance="scope">
        {{ new Intl.NumberFormat("zh-CN", { style: "currency", currency: "CNY" }).format(scope.row.balance) }}
      </template>
      <template #depositTimes="scope"> {{ scope.row.depositTimes }}次 </template>
      <!-- 菜单操作 -->
      <template #operation="scope">
        <el-button type="primary" link :icon="EditPen" @click="createDeposit(scope)"> 充值 </el-button>
      </template>
    </ProTable>
    <ImportExcel ref="dialogRef" />
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";
  import { IColumnProps, ProTableInstance } from "@/components/ProTable/interface";
  import { EditPen, Download } from "@element-plus/icons-vue";
  import ProTable from "@/components/ProTable/index.vue";
  import { useRouter } from "vue-router";
  import { queryPage, exportMembers } from "@/api/member";
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
    {
      prop: "memberName",
      label: "会员名称",
      align: "center",
      search: { el: "input", props: { placeholder: "请输入会员名称" } }
    },
    { prop: "mobile", label: "手机号码", align: "center", search: { el: "input", props: { placeholder: "请输入会员手机号码" } } },
    { prop: "balance", label: "余额", align: "right" },
    { prop: "description", label: "备注", align: "left" },
    { prop: "depositTimes", label: "共充值", minWidth: 100 },
    { prop: "lastDepositedAt", label: "上次充值", minWidth: 100 },
    { prop: "operation", label: "操作", minWidth: 120, fixed: "right" }
  ];

  const createDeposit = scope => router.push({ name: "NewDeposit", state: { id: scope.row.memberId } });
  // 导出用户列表
  const downloadFile = async () => {
    ElMessageBox.confirm("确认导出充值列表吗?", "温馨提示", { type: "warning" }).then(() =>
      useDownload(exportMembers, "充值列表导出", tableRef.value?.searchParameters)
    );
  };
</script>
