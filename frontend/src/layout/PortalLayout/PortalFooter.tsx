import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import { Footer, FooterProps } from 'mada-design-system'
import { Link } from 'react-router-dom'
import { useMemo } from 'react'
import { Facebook, Instgram, Linkedin, X, Youtube } from './SocialIcons'

const PortalFooter = () => {
    // Use useMemo to ensure stable references and proper keys
    const footerConfig = useMemo((): FooterProps => {
        return {
            extraLinks: [
                { 
                    label: strings.footer.TermsAndConditions, 
                    render: <Link key="terms-link" to={pathNames.termsAndConditions}>{strings.footer.TermsAndConditions}</Link> 
                },
                { 
                    label: strings.footer.PolicyAndPrivacy, 
                    render: <Link key="privacy-link" to={pathNames.privacyPolicy}>{strings.footer.PolicyAndPrivacy}</Link> 
                },
            ],
            groupLinks: [
                {
                    links: [],
                    title: 'استكشاف',
                },
                {
                    links: [
                        { 
                            label: strings.faq.title, 
                            render: <Link key="faq-link" to={pathNames.faqs}>{strings.faq.title}</Link> 
                        },
                        { 
                            label: 'طلب تذكرة دعم', 
                            render: <Link key="support-link" to={pathNames.portalSupportCreate}>طلب تذكرة دعم</Link> 
                        },
                        { 
                            label: 'استعلام عن تذكرة', 
                            render: <Link key="inquiry-link" to={pathNames.portalSupportInquiry}>استعلام عن تذكرة</Link> 
                        },
                    ],
                    title: 'المساعدة والدعم',
                },
                {
                    links: [{ 
                        label: 'وزارة التعليم -  المملكة العربية السعودية ', 
                        href: 'https://moe.gov.sa' 
                    }],
                    title: 'روابط مساعدة',
                },
            ],
            showGroupLinks: true,
            socialMediaLinks: [
                {
                    target: 'https://www.linkedin.com/company/ministry-of-education-saudi-arabia',
                    icon: <Linkedin key="linkedin-icon" />,
                },
                {
                    target: 'https://x.com/moe_gov_sa',
                    icon: <X key="twitter-icon" />,
                },
                {
                    target: 'https://www.youtube.com/@moe_gov',
                    icon: <Youtube key="youtube-icon" />,
                },
                {
                    target: 'https://www.facebook.com/moegov.sa',
                    icon: <Facebook key="facebook-icon" />,
                },
                {
                    target: 'https://www.instagram.com/moe_gov_sa',
                    icon: <Instgram key="instagram-icon" />,
                },
            ],
            socialMediaTitle: strings.footer.contactUs,
        }
    }, []);

    return (
        <div key="portal-footer">
            <Footer {...footerConfig} />
        </div>
    )
}

export default PortalFooter
