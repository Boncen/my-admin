import useTabRouter from "@/hooks/tabRouter";
import { Tabs } from "antd";
import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";

type TargetKey = React.MouseEvent | React.KeyboardEvent | string;


const BodyTabs: React.FC = () => {

    const navigate = useNavigate();

    const { tabs, activeKeyFromMatch } = useTabRouter();
    const [activeKey, setActiveKey] = useState(activeKeyFromMatch);
    const [items, setItems] = useState(tabs);

    useEffect(() => {
        setItems([
            ...tabs
        ])
    }, [tabs])
    useEffect(() => {
        setActiveKey(activeKeyFromMatch);
    }, [activeKeyFromMatch])
    const onChange = (key: string) => {
        setActiveKey(key);
        const tab = tabs.find(x=>x.key == key);
        if (tab &&tab.path) {
            navigate(tab.path);
        }
    };
    const removeTab = (targetKey: TargetKey) => {
        const targetIndex = items!.findIndex((pane) => pane.key === targetKey);
        const newPanes = items!.filter((pane) => pane.key !== targetKey);
        if (newPanes.length && targetKey === activeKey) {
            const { key } = newPanes[targetIndex === newPanes.length ? targetIndex - 1 : targetIndex];
            setActiveKey(key);
        }
        setItems(newPanes);
    };
    const onEdit = (targetKey: TargetKey, action: 'add' | 'remove') => {
        if (action === 'add') {
            //   add();
        } else {
            removeTab(targetKey);
        }
    };

    return (
        <div className="h-full">
            <Tabs
                hideAdd
                onChange={onChange}
                activeKey={activeKey}
                type="editable-card"
                onEdit={onEdit}
                items={items}
                className="h-full"
            />
        </div>
    );
}

export default BodyTabs;