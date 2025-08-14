import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import PrivacyPolicyList from '../component/PrivacyPolicyList'

export default function TermsAndConditions() {
    return (
        <div>
            <PortalHeaderPage titlePage={'سياسة الخصوصية'} descriptionPage={'احترام خصوصيتك أولويتنا ولا نستخدم بياناتك دون موافقة مسبقة.'} />

            <PrivacyPolicyList />
        </div>
    )
}
