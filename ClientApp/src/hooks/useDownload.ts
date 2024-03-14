import { ElNotification } from "element-plus";

/**
 * @description 接收数据流生成 blob，创建链接，下载文件
 * @param {Function} api 导出表格的api方法 (必传)
 * @param {String} tempName 导出的文件名 (必传)
 * @param {Object} params 导出的参数 (默认{})
 * @param {Boolean} isNotify 是否有导出消息提示 (默认为 true)
 * @param {String} fileType 导出的文件格式 (默认为.xlsx)
 * */
export const useDownload = async (
  api: (param: any) => Promise<any>,
  tempName: string,
  params: any = {},
  isNotify: boolean = true,
  fileType: string = ".xlsx"
) => {
  if (isNotify) {
    ElNotification({
      title: "温馨提示",
      message: "如果数据庞大会导致下载缓慢哦，请您耐心等待！",
      type: "info",
      duration: 3000
    });
  }
  try {
    const res = await api(params);
    const blobType = "application/force-download";
    const blob = new Blob([res], { type: blobType });
    // 兼容 edge 不支持 createObjectURL 方法
    if ("msSaveOrOpenBlob" in navigator) return window.navigator.msSaveOrOpenBlob(blob, tempName + fileType);
    const blobUrl = window.URL.createObjectURL(blob);
    const fileArchor = document.createElement("a");
    fileArchor.setAttribute("href", blobUrl);
    fileArchor.setAttribute("download", `${tempName}${fileType}`);
    // fileArchor.style.display = "none";
    // fileArchor.download = `${tempName}${fileType}`;
    // fileArchor.href = blobUrl;
    // document.body.appendChild(fileArchor);
    fileArchor.click();
    // 去除下载对 url 的影响
    // document.body.removeChild(fileArchor);
    // window.URL.revokeObjectURL(blobUrl);
  } catch (error) {
    console.log(error);
  }
};
