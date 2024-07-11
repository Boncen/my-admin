

export function getChildren(keyPath: string[], obj: any[]) {
    let list:any[] = obj;
    let target:any = undefined;
    for (let index = 0; index < keyPath.length; index++){
        const key = keyPath[index];
        if (!list || list.length < 1) {
            break;
        }
        target = list.find(x=>x.key === key);
        list = target?.children;
    }
    return target;
}