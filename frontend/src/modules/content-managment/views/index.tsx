import { MainPanel, PageHeader } from '@/shared/components'
import { pathNames } from '@/shared/constants/pathNames'
import { BorderColor, Image, QuestionAnswer, Notifications } from 'google-material-icons/filled'
import { Breadcrumbs, Grid, GridItem, Stack, Card } from 'mada-design-system'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

const ContentManagment = () => {
    const { t } = useTranslation('content-managment');
    
    const contentFeatures = [
        {
            title: t('features.mediaManagement.title'),
            description: t('features.mediaManagement.description'),
            icon: Image,
            href: pathNames.mediaManagement,
            implemented: true,
            stats: '2,847 files • 47.2 GB'
        },
        {
            title: t('features.questionBank.title'),
            description: t('features.questionBank.description'),
            icon: QuestionAnswer,
            href: pathNames.questionBank,
            implemented: true,
            stats: t('features.questionBank.stats')
        },
        {
            title: t('features.notifications.title'),
            description: t('features.notifications.description'),
            icon: Notifications,
            href: pathNames.notifications,
            implemented: true,
            stats: t('features.notifications.stats')
        }
    ]

    return (
        <MainPanel>
            <main className="flex flex-col gap-space-05">
                <Breadcrumbs
                    items={[
                        {
                            title: 'Dashboard',
                            path: pathNames.dashboard,
                        },
                        { title: t('page.breadcrumbTitle') },
                    ]}
                />
                <PageHeader
                    title={t('page.title')}
                    description={t('page.description')}
                    icon={BorderColor}
                    dataTestId="content-management-page-title"
                />
                
                <Grid gap={4} cols={6}>
                    {contentFeatures.map((feature, index) => (
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
                                            {feature.implemented && feature.stats && (
                                                <span className="text-caption-01 text-green-600 font-medium">
                                                    {feature.stats}
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

                {/* Quick Stats Section */}
                <div className="bg-muted/50 rounded-2 p-space-04 mt-space-05">
                    <h2 className="text-subtitle-01 font-bold text-foreground mb-space-03">
                        {t('overview.title')}
                    </h2>
                    <Grid gap={3} cols={12}>
                        <GridItem columns={{ base: 12, md: 4 }}>
                            <div className="text-center">
                                <div className="text-title-01 font-bold text-primary">2,847</div>
                                <div className="text-caption-01 text-foreground-secondary">{t('overview.totalFiles')}</div>
                            </div>
                        </GridItem>
                        <GridItem columns={{ base: 12, md: 4 }}>
                            <div className="text-center">
                                <div className="text-title-01 font-bold text-primary">47.2 GB</div>
                                <div className="text-caption-01 text-foreground-secondary">{t('overview.totalSize')}</div>
                            </div>
                        </GridItem>
                        <GridItem columns={{ base: 12, md: 4 }}>
                            <div className="text-center">
                                <div className="text-title-01 font-bold text-primary">24</div>
                                <div className="text-caption-01 text-foreground-secondary">{t('overview.questionBanks')}</div>
                            </div>
                        </GridItem>
                    </Grid>
                </div>
            </main>
        </MainPanel>
    )
}

export default ContentManagment
