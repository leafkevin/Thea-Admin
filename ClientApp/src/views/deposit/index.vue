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
        <el-button type="primary" link :icon="CirclePlus" @click="createDeposit(scope)"> 充值 </el-button>
        <el-button type="primary" link :icon="EditPen" @click="modifyDeposit(scope)"> 编辑 </el-button>
        <el-button type="danger" link :icon="Delete" @click="deleteDeposit(scope)"> 删除 </el-button>
      </template>
    </ProTable>
    <ImportExcel ref="dialogRef" />
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";
  import { IColumnProps, ProTableInstance } from "@/components/ProTable/interface";
  import { Delete, EditPen, CirclePlus, Download } from "@element-plus/icons-vue";
  import ProTable from "@/components/ProTable/index.vue";
  import { useRouter } from "vue-router";
  import { queryPage, exportDeposits } from "@/api/deposit";
  import { ElMessageBox } from "element-plus";
  import { useDownload } from "@/hooks/useDownload";

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
      search: {
        el: "input",
        props: {
          placeholder: "请输入会员名称",
          onkeyup: async event => {
            if (event.keyCode === 13) await tableRef.value?.search();
          }
        }
      }
    },
    {
      prop: "mobile",
      label: "手机号码",
      align: "center",
      search: {
        el: "input",
        props: {
          placeholder: "请输入会员手机号码",
          onkeyup: async event => {
            if (event.keyCode === 13) await tableRef.value?.search();
          }
        }
      }
    },
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
      useDownload(exportDeposits, "充值列表导出", tableRef.value?.searchParameters)
    );
  };
</script>
