<template>
  <div v-if="loading" class="loading">Loading...</div>
  <div v-else class="card content-box">
    <el-form ref="formRef" :model="ruleForm" :rules="rules" label-width="140px">
      <el-form-item label="会员名称" prop="memberName">
        <el-input v-model="ruleForm.memberName" placeholder="请输入会员名称，必填" clearable />
      </el-form-item>
      <el-form-item label="会员手机号" prop="mobile">
        <el-input v-model="ruleForm.mobile" placeholder="请输入会员手机号码，必填" clearable />
      </el-form-item>
      <el-form-item label="性别">
        <el-radio-group v-model="ruleForm.gender">
          <el-radio-button label="未知" :value="0" />
          <el-radio-button label="男性" :value="1" />
          <el-radio-button label="女性" :value="2" />
        </el-radio-group>
      </el-form-item>
      <el-form-item label="余额" prop="balance">
        <el-input v-model="ruleForm.balance" placeholder="请输入充值余额，必填，并且>0" @focus="parseNumber" @blur="formatText">
          <template #prepend>¥</template>
        </el-input>
      </el-form-item>
      <el-form-item label="描述">
        <el-input v-model="ruleForm.description" placeholder="请输入描述信息" type="textarea" clearable />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm(formRef)"> 保存 </el-button>
        <el-button @click="goBack"> 返回 </el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref, onActivated } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import { checkPhoneNumber } from "@/utils/eleValidate";
  import type { FormInstance, FormRules } from "element-plus";
  import { ElNotification } from "element-plus";
  import { IMemberState, createMember, getMember, modifyMember } from "@/api/member";
  import { useTabPageStore } from "@/stores/tabPages";

  defineOptions({
    name: "MemberEdit"
  });

  const loading = ref(false);
  const formRef = ref<FormInstance>();
  const currentRoute = useRoute();
  const router = useRouter();
  const tabPageStore = useTabPageStore();
  const memberId = history.state.id as string;

  const ruleForm = reactive<IMemberState>({
    memberId: memberId,
    memberName: "",
    mobile: "",
    gender: 0,
    balance: "0.00",
    description: ""
  });
  onActivated(async () => {
    if (memberId && memberId.length > 0) {
      loading.value = true;
      const response = await getMember(memberId);
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
      ruleForm.gender = memberState.gender;
      ruleForm.balance = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(
        memberState.balance as number
      );
      ruleForm.description = memberState.description;
    } else {
      ruleForm.memberId = "";
      ruleForm.memberName = "";
      ruleForm.mobile = "";
      ruleForm.gender = 0;
      ruleForm.balance = "0.00";
      ruleForm.description = "";
    }
    loading.value = false;
  });
  const rules = reactive<FormRules>({
    memberName: [{ required: true, message: "请输入会员名称", trigger: "blur" }],
    mobile: [{ required: true, validator: checkPhoneNumber, trigger: "blur" }],
    balance: [{ required: true, message: "请输入充值余额，必填，并且>0", trigger: "blur" }]
  });

  const submitForm = async (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    await formEl.validate(async (valid, fields) => {
      if (valid) {
        let response;
        if (memberId && memberId.length > 0) response = await modifyMember(ruleForm);
        else response = await createMember(ruleForm);
        if (!response.isSuccess) {
          ElNotification({
            title: "操作失败",
            message: `${response.message}，code:${response.code}`,
            type: "error"
          });
        }
        tabPageStore.removeTabPage(currentRoute.fullPath);
        router.push({ name: "MemberList", replace: true });
        ElNotification({
          title: "操作成功",
          message: `操作成功，返回会员列表`,
          type: "success",
          duration: 3000
        });
      } else {
        console.log("error submit!", fields);
      }
    });
  };
  const parseNumber = () => {
    const formattedValue = ruleForm.balance.toString();
    ruleForm.balance = formattedValue.replace(/\$\s?|(,*)/g, "");
  };
  const formatText = () => {
    const formattedValue = ruleForm.balance.toString();
    const orgiValue = parseFloat(formattedValue);
    ruleForm.balance = new Intl.NumberFormat("zh-CN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(orgiValue);
  };
  const goBack = () => {
    tabPageStore.removeTabPage(currentRoute.fullPath);
    router.push({ name: "MemberList" });
  };
</script>
<style scoped lang="scss">
  @import "./index.scss";
</style>
