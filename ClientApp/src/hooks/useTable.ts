import { ElNotification } from "element-plus";
import { reactive, computed, toRefs } from "vue";

/**
 * @description table 页面操作方法封装
 * @param {Function} api 获取表格数据 api 方法 (必传)
 * @param {Object} initParameters 获取数据初始化参数 (非必传，默认为{})
 * @param {Boolean} isPaging 是否有分页 (非必传，默认为true)
 * @param {Function} dataCallBack 对后台返回的数据进行处理的方法 (非必传)
 * */
export const useTable = (
  api?: (params: any) => Promise<any>,
  initParameters: object = {},
  isPaging: boolean = true,
  //dataCallBack?: (data: any) => any,
  requestError?: (error: any) => void
) => {
  const state = reactive<ITableState>({
    // 表格数据
    tableData: [],
    // 分页数据
    pagination: {
      // 当前页数
      pageIndex: 1,
      // 每页显示条数
      pageSize: 15,
      // 总条数
      total: 0
    },
    // 查询参数(只包括查询)
    searchParameters: {},
    // 初始化默认的查询参数
    searchInitParameters: {},
    // 总参数(包含分页和查询参数)
    totalParameters: {}
  });

  /**
   * @description 分页查询参数(只包括分页和表格字段排序,其他排序方式可自行配置)
   * */
  const pagingParameters = computed({
    get: () => {
      return {
        pageIndex: state.pagination.pageIndex,
        pageSize: state.pagination.pageSize
      };
    },
    set: (newVal: any) => {
      console.log("我是分页更新之后的值", newVal);
    }
  });

  /**
   * @description 获取表格数据
   * @return void
   * */
  const getTableList = async () => {
    if (!api) return;
    try {
      // 先把初始化参数和分页参数放到总参数里面
      Object.assign(state.totalParameters, initParameters, isPaging ? pagingParameters.value : {});
      let response = await api({ ...state.searchInitParameters, ...state.totalParameters });
      if (!response.isSuccess) {
        ElNotification({
          title: "登录失败",
          message: `登录失败，${response.message}，code:${response.code}`,
          type: "error"
        });
      }
      // dataCallBack && (data = dataCallBack(response.data));
      if (isPaging) {
        // 解构后台返回的分页数据 (如果有分页更新分页信息)
        const pagingData = response.data as IPagingData;
        state.tableData = pagingData.data;
        updatePaging({ pageIndex: pagingData.pageIndex + 1, pageSize: state.pagination.pageSize, total: pagingData.totalCount });
      } else state.tableData = response.data as any[];
    } catch (error) {
      requestError && requestError(error);
    }
  };

  /**
   * @description 更新查询参数
   * @return void
   * */
  const updatedTotalParameters = () => {
    state.totalParameters = {};
    // 处理查询参数，可以给查询参数加自定义前缀操作
    let nowSearchParam: ITableState["searchParameters"] = {};
    // 防止手动清空输入框携带参数（这里可以自定义查询参数前缀）
    for (let key in state.searchParameters) {
      // 某些情况下参数为 false/0 也应该携带参数
      if (state.searchParameters[key] || state.searchParameters[key] === false || state.searchParameters[key] === 0) {
        nowSearchParam[key] = state.searchParameters[key];
      }
    }
    Object.assign(state.totalParameters, nowSearchParam, isPaging ? pagingParameters.value : {});
  };

  /**
   * @description 更新分页信息
   * @param {Object} pagingState 后台返回的分页数据
   * @return void
   * */
  const updatePaging = (pagingState: IPagingState) => {
    Object.assign(state.pagination, pagingState);
  };

  /**
   * @description 表格数据查询
   * @return void
   * */
  const search = () => {
    state.pagination.pageIndex = 1;
    updatedTotalParameters();
    getTableList();
  };

  /**
   * @description 表格数据重置
   * @return void
   * */
  const reset = () => {
    state.pagination.pageIndex = 1;
    // 重置搜索表单的时，如果有默认搜索参数，则重置默认的搜索参数
    state.searchParameters = { ...state.searchInitParameters };
    updatedTotalParameters();
    getTableList();
  };

  /**
   * @description 每页条数改变
   * @param {Number} val 当前条数
   * @return void
   * */
  const handleSizeChange = (val: number) => {
    state.pagination.pageIndex = 1;
    state.pagination.pageSize = val;
    getTableList();
  };

  /**
   * @description 当前页改变
   * @param {Number} val 当前页
   * @return void
   * */
  const handleCurrentChange = (val: number) => {
    state.pagination.pageIndex = val;
    getTableList();
  };

  return {
    ...toRefs(state),
    getTableList,
    search,
    reset,
    handleSizeChange,
    handleCurrentChange,
    updatedTotalParam: updatedTotalParameters
  };
};
