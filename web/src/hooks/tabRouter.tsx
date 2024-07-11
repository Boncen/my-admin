import useTabStore from "@/store/tabsStore";
import { useEffect, useState } from "react";
import { useMatches, useOutlet } from "react-router-dom"

export default function useTabRouter() {
    const outlet = useOutlet();
    const matches = useMatches();
    const addTabToStore = useTabStore((state) => state.addTab)
    const storeTabs = useTabStore((state) => state.tabs)
    const [tabs, setTabs] = useState(storeTabs);
    const [activeKeyFromMatch, setActiveKeyFromMatch] = useState(tabs[0]?.key);

    const currentRoute = null;
    useEffect(() => {
        const m = matches[matches.length - 1];
        // if (m && m.pathname) {
        // 从菜单表中获取匹配项

        // 添加到tabStore中
        const t = {
            key: m.pathname,
            label: m.pathname,
            path: m.pathname,
            children: outlet
        }
        addTabToStore(t)
        setActiveKeyFromMatch(m.pathname);
        console.log('tabs', tabs);
        const existsTab = tabs.findIndex(x=>x.key === t.key);
        if (existsTab === -1) {
            tabs.push(t);
        }
        setTabs([
            ...tabs,
        ]);
        // }
    }, [matches]);

    return {
        tabs,
        currentRoute,
        activeKeyFromMatch
    }
} 