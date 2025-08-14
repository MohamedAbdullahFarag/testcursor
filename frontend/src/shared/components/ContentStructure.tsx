import { LocalizedDropdownOption } from '@/shared/models/interfaces/LookupOption'
import parser from 'html-react-parser'
import { Separator, Skeleton, useSwitchData } from 'mada-design-system'
import React from 'react'
import { Else, If, Then } from 'react-if'
interface SectionContent extends LocalizedDropdownOption {
    ContentAr: string
    ContentEn: string
    SubSectionContents?: SectionContent[]
}
interface ContentStructureProps {
    content: SectionContent[]
}
const ContentStructure = ({ content }: ContentStructureProps) => {
    const localize = useSwitchData()
    return (
        <div className="flex flex-col py-space-07">
            <If condition={!content || content?.length < 1}>
                <Then>
                    <ContentStructureSkeleton />
                </Then>
                <Else>
                    <div className="flex h-full flex-1 flex-col gap-space-05 scroll-smooth">
                        {content?.map((item, i) => (
                            <React.Fragment key={item?.id}>
                                <div className="flex flex-col scroll-smooth">
                                    <div className="flex flex-col gap-space-03" id={`${item?.nameEn}${item?.id}`}>
                                        <h1 data-section className="title-01 font-bold">
                                            {localize(item?.nameAr, item?.nameEn)}
                                        </h1>
                                        <div className="custom-font-override break-words text-body-02 [&_li::marker]:text-xs [&_ul]:mr-6 [&_ul]:list-disc [&_ul]:pr-5">
                                            {parser(localize(item?.ContentAr, item?.ContentEn) ?? '')}
                                        </div>
                                    </div>
                                    <ul className="flex flex-col gap-space-05">
                                        {item?.SubSectionContents?.map(section => (
                                            <li
                                                data-section
                                                key={section?.id}
                                                className="flex flex-col gap-space-03"
                                                id={`${section?.nameEn}${section?.id}`}>
                                                <h3 className="text-subtitle-02 font-bold text-primary">
                                                    {localize(section?.nameAr, section?.nameEn)}
                                                </h3>
                                                <div className="custom-font-override break-words text-body-02 [&_li::marker]:text-xs [&_ul]:mr-6 [&_ul]:list-disc [&_ul]:pr-5">
                                                    {parser(localize(section?.ContentAr, section?.ContentEn) ?? '')}
                                                </div>
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                                {i !== content?.length - 1 && <Separator className="my-space-03 xl:my-space-06" />}
                            </React.Fragment>
                        ))}
                    </div>
                </Else>
            </If>
        </div>
    )
}

export const ContentStructureSkeleton = () => {
    return (
        <div className="xl: container relative gap-space-02 px-space-04 py-space-04 xl:flex xl:px-space-09 xl:py-space-05">
            <Skeleton className="hidden h-[300px] w-[200px] py-space-05 xl:flex" />
            <Skeleton className="flex h-[500px] w-full flex-col gap-space-05 xl:pl-5 xl:pr-[224px]" />
        </div>
    )
}

export default ContentStructure
