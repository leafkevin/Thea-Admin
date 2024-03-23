<template>
  <div class="table-box">
    <ProTable
      ref="tableRef"
      title="会员充值列表"
      row-key="depositId"
      :indent="20"
      :columns="columns"
      :request-api="queryPage"
      :search-column-widths="{ xs: 1, sm: 1, md: 2, lg: 3, xl: 3 }"
      highlight-current-row>
      <!-- 表格 header 按钮 -->
      <template #tableHeader>
        <el-button type="primary" :icon="Download" plain @click="downloadFile">导出</el-button>
      </template>
      <template #amount="scope">
        {{ new Intl.NumberFormat("zh-CN", { style: "currency", currency: "CNY" }).format(scope.row.amount) }}
      </template>
      <template #bonus="scope">
        {{ new Intl.NumberFormat("zh-CN", { style: "currency", currency: "CNY" }).format(scope.row.bonus) }}
      </template>
      <template #endBalance="scope">
        {{ new Intl.NumberFormat("zh-CN", { style: "currency", currency: "CNY" }).format(scope.row.endBalance) }}
      </template>
      <template #depositTimes="scope"> {{ scope.row.depositTimes }}次 </template>
      <!-- 菜单操作 -->
      <template #operation="scope">
        <el-button type="primary" link :icon="CirclePlus" @click="createDeposit(scope)"> 充值 </el-button>
        <el-button type="primary" link :icon="EditPen" @click="modifyDeposit(scope)"> 编辑 </el-button>
        <el-button type="danger" link :icon="Delete" @click="deleteDeposit(scope)" v-if="scope.row.isAllowCancel">
          撤销
        </el-button>
      </template>
    </ProTable>
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";
  import { IColumnProps, ProTableInstance } from "@/components/ProTable/interface";
  import { Delete, EditPen, CirclePlus, Download } from "@element-plus/icons-vue";
  import ProTable from "@/components/ProTable/index.vue";
  import { useRouter } from "vue-router";
  import { queryPage, cancelDeposit, exportDeposits } from "@/api/deposit";
  import { ElMessageBox } from "element-plus";
  import { useDownload } from "@/hooks/useDownload";
  import { useHandleData } from "@/hooks/useHandleData";

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
    { prop: "amount", label: "充值金额", align: "right" },
    { prop: "bonus", label: "赠送金额", align: "right" },
    { prop: "endBalance", label: "充值后余额", align: "right" },
    { prop: "description", label: "备注", align: "left" },
    { prop: "createdAt", label: "充值日期", minWidth: 100 },
    { prop: "operation", label: "操作", minWidth: 120, fixed: "right" }
  ];

  const createDeposit = scope => router.push({ name: "DepositEdit", state: { id: scope.row.memberId, mode: "Create", from: 2 } });
  const modifyDeposit = scope => router.push({ name: "DepositEdit", state: { id: scope.row.depositId, mode: "Edit", from: 2 } });
  const deleteDeposit = async scope => {
    const depositAmount = scope.row.amount + scope.row.bonus;
    const formattedValue = new Intl.NumberFormat("zh-CN", {
      style: "currency",
      currency: "CNY",
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(depositAmount);
    await useHandleData(
      cancelDeposit,
      { id: scope.row.depositId },
      `确定要撤销充值吗？本操作将会删除当前充值记录，并扣减会员【${scope.row.memberName}】余额【${formattedValue}】！`
    );
    await tableRef.value?.search();
  };
  // 导出用户列表
  const downloadFile = async () => {
    ElMessageBox.confirm("确认导出充值列表吗?", "温馨提示", { type: "warning" }).then(() =>
      useDownload(exportDeposits, "充值列表导出", tableRef.value?.searchParameters)
    );
  };
</script>
