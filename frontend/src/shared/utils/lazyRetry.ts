/* eslint-disable @typescript-eslint/no-explicit-any */
//
/**
 * Function to retry loading a chunk to avoid chunk load error for out of date code by force reloading window
 * @param importFn function returns Promise of type import() method
 * @param pathModule string incase named exported module
 * @returns Promise<resolved module>
 */
export const lazyRetry = function (importFn: any, pathModule?: string): Promise<any> {
    return new Promise((resolve, reject) => {
        const hasRefreshed = JSON.parse(window.sessionStorage.getItem('retry-lazy-refreshed') ?? 'false')
        importFn()
            .then((component: { [x: string]: any; default: any }) => {
                window.sessionStorage.setItem('retry-lazy-refreshed', 'false') // success so reset the refresh

                resolve(component.default ? component : { default: component[pathModule ?? ''] })
            })
            .catch((error: any) => {
                if (!hasRefreshed) {
                    // not been refreshed yet
                    window.sessionStorage.setItem('retry-lazy-refreshed', 'true') // we are now going to refresh
                    return window.location.reload() // refresh the page
                }
                reject(error) // Default error behavior as already tried refresh
            })
    })
}
