import { create } from 'zustand'


interface TabState {
    tabs: Array<Tab>,
    activeKey?: string,
}

interface TabsAction {
    addTab: (t: Tab) => void,
    rmTab: (key: string) => void,
    rmAll: () => void,
    rmOthers: (key: string) => void,
    rmLeft: (key: string) => void,
    rmRight: (key: string) => void,
    setActive: (key: string) => void,
}


const useTabStore = create<TabState & TabsAction>()((set) => ({
    tabs: [],
    activeKey: '',
    setActive: (key) => set((state) => {
        state.activeKey = key;
        return state;
    }),
    addTab: (t) => set((state) => {
        const tab = state.tabs.find(x => x.key === t.key);
        if (!tab) {
            state.tabs.push(t);
            return state;
        }else {
            state.activeKey = t.key;
        }
        return state;
    }),
    rmTab: (key) => set((state) => {
        const tmp = state;
        const tabIndex = tmp.tabs.findIndex(x => x.key === key);
        if (tabIndex > -1) {
            tmp.tabs.splice(tabIndex, 1);
        }
        return tmp;
    }),
    rmAll: () => set((state) => {
        state.tabs = [];
        return state;
    }),
    rmLeft: (key) => set((state) => {
        const tabIndex = state.tabs.findIndex(x => x.key === key);
        if (tabIndex > -1) {
            state.tabs.splice(0, tabIndex);
        }
        return state;
    }),
    rmRight: (key) => set((state) => {
        const tabIndex = state.tabs.findIndex(x => x.key === key);
        if (tabIndex > -1) {
            state.tabs.splice(tabIndex + 1);
        }
        return state;
    }),
    rmOthers: (key) => set((state) => {
        let tabIndex = state.tabs.findIndex(x => x.key === key);
        if (tabIndex > -1) {
            state.tabs.splice(0, tabIndex);
        }
        tabIndex = state.tabs.findIndex(x => x.key === key);
        if (tabIndex > -1) {
            state.tabs.splice(tabIndex + 1);
        }
        return state
    })
}));

export default useTabStore;