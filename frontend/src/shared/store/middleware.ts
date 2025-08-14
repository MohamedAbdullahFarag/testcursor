import { StateCreator } from 'zustand'
import { DevtoolsOptions, devtools } from 'zustand/middleware'
import { immer } from 'zustand/middleware/immer'

const defualtDevtoolOpts: DevtoolsOptions = {
    name: 'Store',
    enabled: JSON.parse(import.meta.env.VITE_ENABLE_STOREDEVTOOLS),
    serialize: { options: false },
}

const DevtoolsMiddlewares = <T>(
    f: StateCreator<T, [['zustand/devtools', never], ['zustand/immer', never]]>,
    devtoolsOptions: DevtoolsOptions = defualtDevtoolOpts,
) => devtools(immer(f), { ...defualtDevtoolOpts, ...devtoolsOptions })

export default DevtoolsMiddlewares
