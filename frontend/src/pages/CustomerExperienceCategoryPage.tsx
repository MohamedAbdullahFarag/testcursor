import { MainPanel, PageHeader } from '@/shared/components'
import { pathNames } from '@/shared/constants/pathNames'
import { People, Assessment, Support } from 'google-material-icons/filled'
import { Breadcrumbs, Grid, GridItem, Stack, Card } from 'mada-design-system'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

const CustomerExperienceCategoryPage = () => {
    const { t } = useTranslation('categoryPages');
    
    const customerExperienceFeatures = [
        {
            title: t('customerExperience.features.surveys.title'),
            description: t('customerExperience.features.surveys.description'),
            icon: Assessment,
            href: pathNames.customerExperienceSurveys,
            implemented: true
        },
        {
            title: t('customerExperience.features.feedback.title'),
            description: t('customerExperience.features.feedback.description'),
            icon: Support,
            href: pathNames.customerExperienceFeedback,
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
                        { title: t('customerExperience.breadcrumbTitle') },
                    ]}
                />
                <PageHeader
                    title={t('customerExperience.title')}
                    description={t('customerExperience.description')}
                    icon={People}
                    dataTestId="customer-experience-page-title"
                />
                
                <Grid gap={4} cols={6}>
                    {customerExperienceFeatures.map((feature, index) => (
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

export default CustomerExperienceCategoryPage
