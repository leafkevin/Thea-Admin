<template>
  <div class="card content-box">
    <el-form ref="formRef" :model="ruleForm" :rules="rules" label-width="140px">
      <el-form-item label="会员名称" prop="memberName" required>
        <el-input v-model="ruleForm.memberName" placeholder="请输入会员名称，必填" clearable />
        <el-input :type="'hidden'" v-model="ruleForm.memberId" />
      </el-form-item>
      <el-form-item label="会员手机号" prop="mobile" required>
        <el-input v-model="ruleForm.mobile" placeholder="请输入会员手机号码，必填" clearable />
      </el-form-item>
      <el-form-item label="性别" prop="gender">
        <el-radio-group v-model="ruleForm.gender">
          <el-radio-button label="未知" :value="0" />
          <el-radio-button label="男性" :value="1" />
          <el-radio-button label="女性" :value="2" />
        </el-radio-group>
      </el-form-item>
      <el-form-item label="余额" prop="balance" required>
        <template #prepend>¥</template>
        <el-input
          v-model="ruleForm.balance"
          placeholder="请输入充值余额，必填，并且>0"
          :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
          :parser="value => value.replace(/\s?|(,*)/g, '')" />
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input v-model="ruleForm.description" placeholder="请输入描述信息" type="textarea" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm(formRef)"> 保存 </el-button>
        <el-button @click="resetForm(formRef)"> 重置 </el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref } from "vue";
  import { checkPhoneNumber } from "@/utils/eleValidate";
  import type { FormInstance, FormRules } from "element-plus";
  import { ElMessage } from "element-plus";
  //import { IMemberState, createMember, modifyMember } from "@/api/member";

  const formRef = ref<FormInstance>();
  const ruleForm = ref<IMemberState>({
    memberId: "",
    memberName: "",
    mobile: "",
    gender: 0,
    balance: 0.0,
    description: ""
  });

  const rules = reactive<FormRules>({
    memberName: [{ required: true, message: "请输入会员名称", trigger: "blur" }],
    mobile: [{ required: true, validator: checkPhoneNumber, trigger: "blur" }],
    balance: [{ required: true, message: "请输入充值余额，必填，并且>0", trigger: "blur" }]
  });

  const submitForm = async (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    await formEl.validate((valid, fields) => {
      if (valid) {
        //await createMember();
        ElMessage.success("提交的数据为 : " + JSON.stringify(ruleForm));
      } else {
        console.log("error submit!", fields);
      }
    });
  };

  const resetForm = (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    formEl.resetFields();
  };
</script>
<style scoped lang="scss">
  @import "./index.scss";
</style>
