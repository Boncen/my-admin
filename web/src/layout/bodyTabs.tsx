import { Tabs } from "antd";
import React, { useEffect, useRef, useState } from 'react';
import { useOutlet } from "react-router-dom";

type TargetKey = React.MouseEvent | React.KeyboardEvent | string;


const BodyTabs = () => {
    const defaultPanes = new Array(2).fill(null).map((_, index) => {
        const id = String(index + 1);
        return { label: `Tab ${id}`, children: `Content of Tab Pane ${index + 1}`, key: id };
    });
    const [activeKey, setActiveKey] = useState(defaultPanes[0].key);
    const [items, setItems] = useState(defaultPanes);

    const outlet = useOutlet();
    useEffect(()=>{
        console.log('outlet change',outlet);
    },[outlet])
    const onChange = (key: string) => {
        setActiveKey(key);
    };
    const remove = (targetKey: TargetKey) => {
        const targetIndex = items.findIndex((pane) => pane.key === targetKey);
        const newPanes = items.filter((pane) => pane.key !== targetKey);
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
            remove(targetKey);
        }
    };
    return (
        <div>

            <Tabs
                hideAdd
                onChange={onChange}
                activeKey={activeKey}
                type="editable-card"
                onEdit={onEdit}
                items={items}
            />
        </div>
    );
}

export default BodyTabs;