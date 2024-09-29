import axios from 'axios';
import { postQuery, QueryObject } from './easy';

export interface LoginData {
  account: string;
  password: string;
}

export interface LoginRes {
  token: string;
}
export function login(data: LoginData) {
  return axios.post('/api/user/login', data);
}

export function logout() {
  return axios.post<LoginRes>('/api/user/logout');
}

export function getUserInfo() {
  const data: QueryObject = {
    "user": {
      "@page": 1,
      "@count": 1,
      "@columns": "MaUser.*, MaRole.Name as role",
      "@join":[
        {
          targetJoin:"UserRole",
          joinField: "UserId",
          onField: "Id",
          targetOn: "MaUser"
        },
        {
          targetJoin:"MaRole",
          joinField: "Id",
          onField: "RoleId",
          targetOn: "UserRole"
        }
      ],
      "@where": {
        "id":{
          value: "$CURRENT_USER_ID$"
        }
      }
    }
  }
  return postQuery(data);
  // return axios.post('/api/user/info');
}

export function getMenuList() {
  return axios.get('/api/user/menus');
  // return axios.post<RouteRecordNormalized[]>('/api/user/menus');
}
