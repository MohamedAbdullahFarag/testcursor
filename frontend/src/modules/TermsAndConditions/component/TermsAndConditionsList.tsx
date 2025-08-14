import ContentStructure from '@/shared/components/ContentStructure'
import PortalFooterPage from '@/shared/components/PortalFooterPage'
import data from '../constants/data.json'
const TermsAndConditionsList = () => {
    return (
        <div className="mx-auto max-w-container">
            <ContentStructure content={data?.SubSectionContents} key={'privacy'} />

            <PortalFooterPage modifiedUtc={data.ModifiedUtc} />

            {/* <ContentSurvey actionPage="FAQ" version={data?.Version} /> */}
        </div>
    )
}

export default TermsAndConditionsList
