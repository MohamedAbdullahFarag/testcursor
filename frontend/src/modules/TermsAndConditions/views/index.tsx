import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import TermsAndConditionsList from '../component/TermsAndConditionsList'

export default function TermsAndConditions() {
    return (
        <div className="">
            <PortalHeaderPage
                titlePage={'الشروط والأحكام'}
                descriptionPage={'توضح الشروط والأحكام السياسات والقواعد المتبعة لتنظيم لاستخدام اختبار.'}
            />

            <TermsAndConditionsList />
        </div>
    )
}
