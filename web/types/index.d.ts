interface Tab {
    label: string,
    key: string,
    path: string,
    children?: ReactNode,
}

interface FCProps {
    children: ReactNode
}

interface Setting {
    isDarkMode: boolean,
    lang?:string,
    isUseMultitab: boolean
}