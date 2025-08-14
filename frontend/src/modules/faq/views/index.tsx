import { strings } from '@/shared/locales'

import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import FAQList from '../components/FAQList'

const Faq = () => {
    return (
        <div>
            <PortalHeaderPage titlePage={strings.faq.title} descriptionPage={strings.faq.faqDescription} />
            <FAQList />
        </div>
    )
}

export default Faq
