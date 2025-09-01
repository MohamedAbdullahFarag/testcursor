import { MainPanel, PageHeader } from '@/shared/components'
import { pathNames } from '@/shared/constants/pathNames'
import { Help, QuestionAnswer, Description, Shield } from 'google-material-icons/filled'
import { Breadcrumbs, Grid, GridItem, Stack, Card } from 'mada-design-system'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

const HelpCategoryPage = () => {
    const { t } = useTranslation('categoryPages');
    
    const helpFeatures = [
        {
            title: t('help.features.faq.title'),
            description: t('help.features.faq.description'),
            icon: QuestionAnswer,
            href: pathNames.helpFaq,
            implemented: true
        },
        {
            title: t('help.features.terms.title'),
            description: t('help.features.terms.description'),
            icon: Description,
            href: pathNames.helpTerms,
            implemented: true
        },
        {
            title: t('help.features.privacy.title'),
            description: t('help.features.privacy.description'),
            icon: Shield,
            href: pathNames.helpPrivacy,
            implemented: true
        }
    ]

    return (
        <MainPanel>
            <main className="flex flex-col gap-space-05">
                <Breadcrumbs
                    items={[
                        {
                            title: t('common.dashboard'),
                            path: pathNames.dashboard,
                        },
                        { title: t('help.breadcrumbTitle') },
                    ]}
                />
                <PageHeader
                    title={t('help.title')}
                    description={t('help.description')}
                    icon={Help}
                    dataTestId="help-page-title"
                />
                
                <Grid gap={4} cols={6}>
                    {helpFeatures.map((feature, index) => (
                        <GridItem key={index} columns={{ base: 6, lg: 3, xl: 2 }}>
                            <Link to={feature.href} className="block h-full">
                                <Card className="h-full hover:shadow-lg transition-shadow duration-200 p-space-03">
                                    <Stack direction="row" alignItems="center" gap={3} className="mb-3">
                                        <div className="rounded-1 bg-primary-50 p-space-02">
                                            <feature.icon className="size-5 text-primary-600" />
                                        </div>
                                        <div className="flex-1">
                                            <h3 className="text-body-01 font-bold text-foreground">
                                                {feature.title}
                                            </h3>
                                            {!feature.implemented && (
                                                <span className="text-caption-01 text-yellow-600 font-medium">
                                                    Coming Soon
                                                </span>
                                            )}
                                        </div>
                                    </Stack>
                                    <p className="text-caption-01 text-foreground-secondary">
                                        {feature.description}
                                    </p>
                                </Card>
                            </Link>
                        </GridItem>
                    ))}
                </Grid>
            </main>
        </MainPanel>
    )
}

export default HelpCategoryPage
