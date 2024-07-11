import React, { useState } from 'react';
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
} from '@ant-design/icons';
import MenuLayout from './menu';
import { Button, Layout, theme } from 'antd';
// import { Outlet } from 'react-router-dom';
import BodyTabs from './bodyTabs';

const { Header, Sider, Content } = Layout;

const App: React.FC = () => {
  const [collapsed, setCollapsed] = useState(false);
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  return (
    <Layout className='h-screen'>
      <Sider trigger={null} collapsible collapsed={collapsed}>
        <div className="demo-logo-vertical" >Logo</div>
        <MenuLayout></MenuLayout>
      </Sider>
      <Layout>
        <Header style={{ padding: 0, background: colorBgContainer }}>
          <Button
            type="text"
            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            onClick={() => setCollapsed(!collapsed)}
            style={{
              fontSize: '16px',
              width: 64,
              height: 64,
            }}
          />
        </Header>
        <Content
          style={{
            margin: '10px 8px',
            // background: colorBgContainer,
            // borderRadius: borderRadiusLG,
          }}
        >
          {/* <Outlet /> */}
          <BodyTabs></BodyTabs>
        </Content>
      </Layout>
    </Layout>
  );
};

export default App;