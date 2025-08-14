import { strings } from '@/shared/locales'
import parser from 'html-react-parser'
import {
    Accordion,
    AccordionContent,
    AccordionItem,
    AccordionTrigger,
    SearchInput,
    Tabs,
    TabsContent,
    TabsList,
    TabsTrigger,
    useLanguage,
    useSwitchData,
} from 'mada-design-system'
import { useEffect, useMemo, useState } from 'react'
import { Default, Switch } from 'react-if'

import PortalFooterPage from '@/shared/components/PortalFooterPage'
import data from '../constants/data.json'

type Section = {
    Id: string
    NameAr: string
    NameEn: string
    ContentAr: string
    ContentEn: string
    IsMobile: boolean
    SubSectionContents: {
        Id: string
        NameAr: string
        NameEn: string
        ContentAr: string
        ContentEn: string
        IsMobile: boolean
    }[]
    DisplayOrder?: number | null
}

const hasValue = (data: object, searchValue: string) => {
    let val = false
    const lowerSearchValue = searchValue.toLowerCase()

    for (const [, value] of Object.entries(data)) {
        if (typeof value === 'string' && value.toLowerCase().includes(lowerSearchValue)) {
            val = true
        } else if (Array.isArray(value)) {
            value?.map(item => {
                for (const [, itemValue] of Object.entries(item)) {
                    if (typeof itemValue === 'string' && itemValue.toLowerCase().includes(lowerSearchValue)) {
                        val = true
                        return
                    }
                }
            })
        }
    }

    return val
}

const FAQList = () => {
    const { dir } = useLanguage()
    const localize = useSwitchData()
    // const { data, isLoading, refetch, isError, error } = useGetContent({ id: ContentCategoriesAlias.FAQ, queryString: '?isMobile=false' })

    const [searchValue, setSearchValue] = useState('')

    const [filteredFAQ, setFilteredFAQ] = useState<Section[]>([])
    const [selectedTab, setSelectedTab] = useState<string>(String(0))

    useEffect(() => {
        setFilteredFAQ(data?.SubSectionContents)
    }, [])

    const findSingleFAQ = useMemo(() => {
        return filteredFAQ?.find(item => String(item?.Id) === selectedTab)
    }, [selectedTab, filteredFAQ])

    const handleSearchChange = (value: string) => {
        setSearchValue(value)
        const newFAQ = data.SubSectionContents.filter(item => {
            return hasValue(item, value)
        })?.map(i => {
            return {
                ...i,
                SubSectionContents: i?.SubSectionContents?.filter(item => {
                    return hasValue(item, value)
                }),
            }
        })
        setFilteredFAQ(newFAQ ?? [])
    }

    return (
        <div className="mx-auto max-w-container">
            <div className="flex flex-col gap-space-06 py-space-07">
                <SearchInput
                    type="onType"
                    placeholder={'بحث'}
                    value={searchValue}
                    rounded="default"
                    onSearch={handleSearchChange}
                    className="w-[320px] max-sm:w-full"
                />
                <Tabs dir={dir} value={selectedTab} onValueChange={setSelectedTab}>
                    <TabsList variant={'underline'} underline>
                        <TabsTrigger className="gap-space-01 !p-space-04 !text-body-02" value={String(0)} data-testid="allTabTest">
                            {strings.faq.all}
                        </TabsTrigger>
                        {data?.SubSectionContents?.map(item => (
                            <TabsTrigger
                                key={item?.Id}
                                className="gap-space-01 !p-space-04 !text-body-02"
                                value={String(item?.Id)}
                                data-testid="FAQTabTest">
                                {localize(item?.NameAr, item?.NameEn)}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                    <TabsContent value={String(0)}>
                        <Switch>
                            <Default>
                                {filteredFAQ?.map(item => (
                                    <div key={item?.Id} data-section id={`${item?.NameEn}${item?.Id}`}>
                                        <Accordion type="single" collapsible defaultValue={filteredFAQ?.[0].SubSectionContents?.[0]?.Id?.toString()}>
                                            {item?.SubSectionContents?.map(section => (
                                                <AccordionItem value={section?.Id?.toString()} key={section?.Id}>
                                                    <AccordionTrigger className="!p-space-04 !text-start">
                                                        <p className="text-body-02 font-bold">{localize(section?.NameAr, section?.NameEn)}</p>
                                                    </AccordionTrigger>
                                                    <AccordionContent className="break-words">
                                                        {parser(localize(section?.ContentAr, section?.ContentEn) ?? '')}
                                                    </AccordionContent>
                                                </AccordionItem>
                                            ))}
                                        </Accordion>
                                    </div>
                                ))}
                            </Default>
                        </Switch>
                    </TabsContent>
                    {data?.SubSectionContents?.map(section => (
                        <TabsContent value={String(section?.Id)} key={section?.Id}>
                            <Switch>
                                <Default>
                                    <Accordion type="single" collapsible defaultValue={findSingleFAQ?.SubSectionContents?.[0]?.Id?.toString()}>
                                        {findSingleFAQ?.SubSectionContents?.map(item => (
                                            <AccordionItem value={item?.Id?.toString()} key={item?.Id}>
                                                <AccordionTrigger className="!p-space-04 !text-start">
                                                    <p className="text-body-02 font-bold">{localize(item?.NameAr, item?.NameEn)}</p>
                                                    {/* <ShowDetails /> */}
                                                </AccordionTrigger>
                                                <AccordionContent className="break-words">
                                                    {parser(localize(item?.ContentAr, item?.ContentEn) ?? '')}
                                                </AccordionContent>
                                            </AccordionItem>
                                        ))}
                                    </Accordion>
                                </Default>
                            </Switch>
                        </TabsContent>
                    ))}
                </Tabs>
            </div>

            <PortalFooterPage modifiedUtc={data.ModifiedUtc} actionPage="FAQ" />
        </div>
    )
}

export default FAQList

// const FAQSkeleton = () => {
//     return (
//         <div className="flex max-h-fit flex-col gap-space-06 px-space-06 py-space-07 2xl:!container max-sm:px-space-04 xl:px-space-10">
//             <Skeleton className="h-[40px] w-[240px] rounded-sm max-sm:w-full" />
//             <div className="flex items-center gap-space-01 max-sm:justify-center">
//                 {Array.from({ length: 5 })?.map(() => <Skeleton key={uuidv4()} className="h-[30px] w-[120px] max-sm:w-full" />)}
//             </div>
//             <div className="flex flex-col gap-space-02">
//                 {Array.from({ length: 4 })?.map(() => <Skeleton key={uuidv4()} className="h-[70px] w-full" />)}
//             </div>
//         </div>
//     )
// }
