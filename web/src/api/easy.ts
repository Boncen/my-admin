import axios from "axios";

const easyUrl = "/api/easy";

export interface JoinCondition {
  targetJoin: string;
  joinField: string;
  onField: string;
  targetOn?: string; // 可选字段
}

export interface WhereConditionValue {
  type?:
    | "contains"
    | "in"
    | "lessThan"
    | "greaterThan"
    | "lessThanOrEqual"
    | "greaterThanOrEqual"
    | "equal"
    | "notEqual"; // 可选字段，用于条件类型
  value: any; // 条件值，可以是任何类型
}

export interface OrderCondition {
  field: string;
  type?: "ASC" | "DESC"; // 排序类型，只允许 asc 或 desc
}

export interface AndCondition {
  [key: string]: WhereConditionValue;
}
interface OrCondition {
  [key: string]: WhereConditionValue;
}

export interface ChildrenCondition {
  target?: string;
  targetField: string;
  parentField: string;
  page?: number;
  count?: number;
  customResultFieldName?: string;
  columns?: string;
}

export interface PostQuery {
  "@page"?: number; // 当前页码
  "@count"?: number; // 每页数量
  "@total"?: 0; // 是否返回total
  "@columns"?: string; // 查询列名，以逗号分隔
  "@join"?: JoinCondition[]; // 联表条件数组
  "@where"?:
    | { "@and"?: AndCondition; "@or"?: OrCondition }
    | { [key: string]: WhereConditionValue }; // 查询条件对象
  "@order"?: OrderCondition[]; // 排序条件数组
  "@children"?: ChildrenCondition;
}

export interface QueryObject {
  [key: string]: PostQuery;
}
/**
 * 获取更新参数
 * @param input
 * @returns
 */
function getUpdateMethodBody(
  input: [
    {
      target: string;
      ids?: string;
      where: { [key: string]: WhereConditionValue };
      data: { [key: string]: WhereConditionValue };
    }
  ]
) {
  const body: any[] = [];
  for (let index = 0; index < input.length; index++) {
    const element = input[index];
    let tmp = {
      "@target": element.target,
      "@where": element.where,
      "@ids": element.ids,
    };
    body.push({ ...element.data, ...tmp });
  }
  return body;
}

/**
 * 更新
 * @param input
 * @returns
 */
export function update(
  input: [
    {
      target: string;
      ids?: string;
      where: { [key: string]: WhereConditionValue };
      data: { [key: string]: WhereConditionValue };
    }
  ]
) {
  const body = getUpdateMethodBody(input);
  return axios.put(easyUrl, body);
}

/**
 * 批量添加数据
 * @param target
 * @param items
 * @returns
 */
export function addMany(target: string, items: [{ [key: string]: any }]) {
  const data: { [key: string]: Array<{ [key: string]: any }> } = {};
  data[target] = items;
  return axios.post(easyUrl, data);
}
/**
 * 添加数据
 * @param target
 * @param items
 * @returns
 */
export function add(target: string, items: { [key: string]: any }) {
  const data: { [key: string]: { [key: string]: any } } = {};
  data[target] = items;
  return axios.post(easyUrl, data);
}
/**
 * 删除
 * @param target
 * @param ids
 * @returns
 */
export function remove(target: string, ids: string) {
  return axios.delete(easyUrl + `?target=${target}&ids=${ids}`);
}

/**
 * post查询
 * @param input
 * @returns
 */
export function postQuery(input: QueryObject) {
  return axios.post(easyUrl, input);
}
/**
 * get查询
 * @param target
 * @param page
 * @param count
 * @param orderField
 * @param orderType
 * @returns
 */
export function query(
  target: string,
  page?: number,
  count?: number,
  columns?: string,
  orderField?: string,
  orderType?: "ASC" | "DESC"
) {
  let queryParam = `?@target=${target}`;
  if (page) {
    queryParam += `&@page=${page}`;
  }
  if (count) {
    queryParam += `&@count=${count}`;
  }
  if (columns) {
    queryParam += `&@columns=${columns}`;
  }
  if (orderField) {
    if (orderType == "ASC") {
      queryParam += `&@orderasc=${orderField}`;
    } else {
      queryParam += `&@orderdesc=${orderField}`;
    }
  }
  return axios.get(easyUrl + queryParam);
}
