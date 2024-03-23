<template>
  <div v-if="loading" class="loading">Loading...</div>
  <div v-else class="card content-box">
    <el-form ref="formRef" :model="ruleForm" :rules="rules" label-width="140px">
      <el-form-item label="会员名称" prop="memberName">
        <el-input v-model="ruleForm.memberName" disabled />
      </el-form-item>
      <el-form-item label="会员手机号" prop="mobile">
        <el-input v-model="ruleForm.mobile" disabled />
      </el-form-item>
      <el-form-item :label="beginBalanceLabel" prop="beginBalance">
        <el-input v-model="ruleForm.beginBalance" disabled>
          <template #prepend>¥</template>
        </el-input>
      </el-form-item>
      <el-form-item label="充值金额" prop="amount">
        <el-input
          v-model="ruleForm.amount"
          placeholder="请输入充值余额，必填，并且>0"
          @focus="parseNumber('amount')"
          @blur="formatText('amount')"
          @change="changeAmount">
          <template #prepend>¥</template>
        </el-input>
      </el-form-item>
      <el-form-item label="赠送金额" prop="bonus">
        <el-input
          v-model="ruleForm.bonus"
          placeholder="请输入赠送余额，必填，并且>0"
          @focus="parseNumber('bonus')"
          @blur="formatText('bonus')"
          @change="changeAmount">
          <template #prepend>¥</template>
        </el-input>
      </el-form-item>
      <el-form-item label="充值后余额" prop="endBalance">
        <el-input v-model="ruleForm.endBalance" disabled>
          <template #prepend>¥</template>
        </el-input>
      </el-form-item>
      <el-form-item label="备注">
        <el-input
          v-model="ruleForm.description"
          placeholder="请输入备注信息"
          type="textarea"
          :autosize="{ minRows: 3, maxRows: 5 }"
          clearable />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm(formRef)"> 保存 </el-button>
        <el-button @click="goBack"> 返回 </el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref, onActivated, computed } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import type { FormInstance, FormRules } from "element-plus";
  import { ElNotification } from "element-plus";
  import { IDepositState, getDeposit, createDeposit, modifyDeposit } from "@/api/deposit";
  import { IMemberState, getMember } from "@/api/member";

  import { useTabPageStore } from "@/stores/tabPages";

  defineOptions({
    name: "DepositEdit"
  });

  const loading = ref(false);
  const formRef = ref<FormInstance>();
  const currentRoute = useRoute();
  const router = useRouter();
  const tabPageStore = useTabPageStore();
  const id = history.state.id as string;
  const from = history.state.from as number;
  const isEdit = computed(() => (history.state.mode as string) === "Edit");
  const beginBalanceLabel = computed(() => (isEdit.value ? "充值前余额" : "当前余额"));

  const ruleForm = reactive<IDepositState>({
    depositId: "",
    memberId: "",
    memberName: "",
    mobile: "",
    beginBalance: "0.00",
    amount: "0.00",
    bonus: "0.00",
    endBalance: "0.00",
    description: ""
  });
  onActivated(async () => {
    loading.value = true;
    if (isEdit.value) {
      const response = await getDeposit(id);
      if (!response.isSuccess) {
        loading.value = false;
        ElNotification({
          title: "获取充值信息失败",
          message: `${response.message}，code:${response.code}`,
          type: "error"
        });
        return;
      }
      const depositState = response.data as IDepositState;
      ruleForm.depositId = depositState.depositId;
      ruleForm.memberId = depositState.memberId;
      ruleForm.memberName = depositState.memberName;
      ruleForm.mobile = depositState.mobile;
      ruleForm.beginBalance = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
        depositState.beginBalance as number
      );
      ruleForm.amount = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
        depositState.amount as number
      );
      ruleForm.bonus = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
        depositState.bonus as number
      );
      ruleForm.endBalance = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
        depositState.endBalance as number
      );
      ruleForm.description = depositState.description;
    } else {
      const response = await getMember(id);
      if (!response.isSuccess) {
        loading.value = false;
        ElNotification({
          title: "获取会员信息失败",
          message: `${response.message}，code:${response.code}`,
          type: "error"
        });
        return;
      }
      const memberState = response.data as IMemberState;
      ruleForm.memberId = memberState.memberId as string;
      ruleForm.memberName = memberState.memberName;
      ruleForm.mobile = memberState.mobile;
      const beginBalance = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
        memberState.balance as number
      );
      ruleForm.beginBalance = beginBalance;
      ruleForm.amount = "0.00";
      ruleForm.bonus = "0.00";
      ruleForm.endBalance = beginBalance;
      ruleForm.description = "";
    }
    loading.value = false;
  });
  const rules = reactive<FormRules>({
    amount: [{ required: true, message: "请输入充值余额，必填，并且>0", trigger: "blur" }]
  });
  const submitForm = async (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    await formEl.validate(async (valid, fields) => {
      if (valid) {
        let response;
        if (isEdit.value) response = await modifyDeposit(ruleForm);
        else response = await createDeposit(ruleForm);
        if (!response.isSuccess) {
          ElNotification({
            title: "操作失败",
            message: `${response.message}，code:${response.code}`,
            type: "error"
          });
        }
        tabPageStore.removeTabPage(currentRoute.fullPath);
        router.push({ name: "DepositList", replace: true });
        ElNotification({
          title: "操作成功",
          message: "操作成功，返回列表页面",
          type: "success",
          duration: 3000
        });
      } else {
        console.log("error submit!", fields);
      }
    });
  };
  const parseNumber = source => {
    if (source === "amount") {
      const formattedValue = ruleForm.amount.toString();
      ruleForm.amount = formattedValue.replace(/\$\s?|(,*)/g, "").replace(".00", "");
    } else {
      const formattedValue = ruleForm.bonus.toString();
      ruleForm.bonus = formattedValue.replace(/\$\s?|(,*)/g, "").replace(".00", "");
    }
  };
  const formatText = source => {
    if (source === "amount") {
      const formattedValue = ruleForm.amount.toString();
      const orgiValue = parseFloat(formattedValue);
      ruleForm.amount = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(orgiValue);
    } else {
      const formattedValue = ruleForm.bonus.toString();
      const orgiValue = parseFloat(formattedValue);
      ruleForm.bonus = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(orgiValue);
    }
  };
  const changeAmount = () => {
    const formattedBeginBalance = ruleForm.beginBalance.toString();
    const beginBalance = parseFloat(formattedBeginBalance.replace(/\$\s?|(,*)/g, ""));
    const formattedAmount = ruleForm.amount.toString();
    const amount = parseFloat(formattedAmount.replace(/\$\s?|(,*)/g, ""));
    const formattedBonus = ruleForm.bonus.toString();
    const bonus = parseFloat(formattedBonus.replace(/\$\s?|(,*)/g, ""));
    ruleForm.endBalance = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
      beginBalance + amount + bonus
    );
  };
  const goBack = () => {
    tabPageStore.removeTabPage(currentRoute.fullPath);
    if (from == 1) router.push({ name: "MemberList" });
    else router.push({ name: "DepositList" });
  };
</script>
<style scoped lang="scss">
  @import "./index.scss";
</style>
