import { UploadOutlined, UserOutlined, VideoCameraOutlined } from "@ant-design/icons";
import { Menu } from "antd";
import { useNavigate } from "react-router-dom";

const MenuLayout: React.FC = () => {
    const navigate = useNavigate();
    const handleMenuSelect = (info: any) => {
        console.log(1, info);
        if (info) {
            navigate("/" + info.key);
        }
    }
    return (
        <div>
            <Menu
                theme="dark"
                mode="inline"
                defaultSelectedKeys={['1']}
                onSelect={handleMenuSelect}
                items={[
                    {
                        key: 'a',
                        icon: <UserOutlined />,
                        label: 'nav 1',
                    },
                    {
                        key: 'aa',
                        icon: <VideoCameraOutlined />,
                        label: 'nav 2',
                    },
                    {
                        key: 'aaa',
                        icon: <UploadOutlined />,
                        label: 'nav 3',
                    },
                ]}
            />
        </div>
    )
}

export default MenuLayout;