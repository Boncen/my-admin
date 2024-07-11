import useTabRouter from "@/hooks/tabRouter";
import { getChildren } from "@/utils";
import { UploadOutlined, UserOutlined, VideoCameraOutlined } from "@ant-design/icons";
import { Menu } from "antd";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const MenuLayout: React.FC = () => {
    const menuItem = [
        {
            key: '/a',
            icon: <UserOutlined />,
            label: 'nav 1',
        },
        {
            key: '/aa',
            icon: <VideoCameraOutlined />,
            label: 'nav 2',
            children: [
                {
                    key: '/aa/aa1',
                    label: 'nav 2-1'
                },
                {
                    key: '/aa/aa2',
                    label: 'nav 2-2'
                }
            ]
        },
        {
            key: '/aaa',
            icon: <UploadOutlined />,
            label: 'nav 3',
        },
    ]
    const { activeKeyFromMatch } = useTabRouter();
    const [selectedKey, setSelectedKey] = useState([menuItem[0]?.key]);
    
    const navigate = useNavigate();
    
    useEffect(()=> {
        setSelectedKey([activeKeyFromMatch])
    }, [activeKeyFromMatch])
    
    const handleMenuSelect = (info: any) => {
        if (info) {
            const item = getChildren(info.keyPath.reverse(), menuItem)
            navigate(item.key);
        }
    }
    return (
        <div>
            <Menu
                theme="dark"
                mode="inline"
                defaultSelectedKeys={[menuItem[0].key]}
                selectedKeys={selectedKey}
                onSelect={handleMenuSelect}
                items={menuItem}
            />
        </div>
    )
}

export default MenuLayout;