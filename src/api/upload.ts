import { http } from "@/api/http";

/**
 * @name 文件上传模块
 */
// 图片上传
export const uploadImage = (params: FormData) => {
  return http.post("/file/uploadImage", params);
};

// 视频上传
export const uploadVideo = (params: FormData) => {
  return http.post("/file/uploadVideo", params);
};
