import { MainPanel, PageHeader } from '@/shared/components'
import { pathNames } from '@/shared/constants/pathNames'
import { Assessment, People, LibraryBooks } from 'google-material-icons/filled'
import { Breadcrumbs, Grid, GridItem, Stack, Card } from 'mada-design-system'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

const AnalyticsCategoryPage = () => {
    const { t } = useTranslation('categoryPages');
    
    const analyticsFeatures = [
        {
            title: t('analytics.features.dashboard.title'),
            description: t('analytics.features.dashboard.description'),
            icon: Assessment,
            href: pathNames.analyticsDashboard,
            implemented: false
        },
        {
            title: t('analytics.features.users.title'),
            description: t('analytics.features.users.description'),
            icon: People,
            href: pathNames.analyticsUsers,
            implemented: false
        },
        {
            title: t('analytics.features.content.title'),
            description: t('analytics.features.content.description'),
            icon: LibraryBooks,
            href: pathNames.analyticsContent,
            implemented: false
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
                        { title: t('analytics.breadcrumbTitle') },
                    ]}
                />
                <PageHeader
                    title={t('analytics.title')}
                    description={t('analytics.description')}
                    icon={Assessment}
                    dataTestId="analytics-page-title"
                />
                
                <Grid gap={4} cols={6}>
                    {analyticsFeatures.map((feature, index) => (
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
                                                    {t('common.comingSoon')}
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

export default AnalyticsCategoryPage
