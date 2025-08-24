import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { 
  Tabs, 
  TabsContent, 
  TabsList, 
  TabsTrigger,
  Card, 
  CardContent, 
  CardHeader, 
  CardTitle,
  Button
} from 'mada-design-system';
import { 
  Upload, 
  FolderOpen, 
  Image, 
  Video, 
  FileText, 
  Archive,
  Settings,
  BarChart3
} from 'lucide-react';

import { MediaLibrary } from './MediaLibrary';
import { MediaUpload } from './MediaUpload';
import { MediaCategories } from './MediaCategories';
import { MediaCollections } from './MediaCollections';
import { MediaAnalytics } from './MediaAnalytics';
import { MediaSettings } from './MediaSettings';

export const MediaManagementPage: React.FC = () => {
  const { t } = useTranslation('media-management');
  const [activeTab, setActiveTab] = useState('library');

  const tabs = [
    {
      id: 'library',
      label: t('tabs.library'),
      icon: <Image className="w-4 h-4" />,
      component: <MediaLibrary />
    },
    {
      id: 'upload',
      label: t('tabs.upload'),
      icon: <Upload className="w-4 h-4" />,
      component: <MediaUpload />
    },
    {
      id: 'categories',
      label: t('tabs.categories'),
      icon: <FolderOpen className="w-4 h-4" />,
      component: <MediaCategories />
    },
    {
      id: 'collections',
      label: t('tabs.collections'),
      icon: <Archive className="w-4 h-4" />,
      component: <MediaCollections />
    },
    {
      id: 'analytics',
      label: t('tabs.analytics'),
      icon: <BarChart3 className="w-4 h-4" />,
      component: <MediaAnalytics />
    },
    {
      id: 'settings',
      label: t('tabs.settings'),
      icon: <Settings className="w-4 h-4" />,
      component: <MediaSettings />
    }
  ];

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
            {t('page.title')}
          </h1>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            {t('page.description')}
          </p>
        </div>
        
        {/* Quick Actions */}
        <div className="flex items-center space-x-3">
          <Button variant="outline" size="sm">
            <Upload className="w-4 h-4 mr-2" />
            {t('actions.quickUpload')}
          </Button>
          <Button variant="outline" size="sm">
            <FolderOpen className="w-4 h-4 mr-2" />
            {t('actions.newCategory')}
          </Button>
          <Button variant="outline" size="sm">
            <Archive className="w-4 h-4 mr-2" />
            {t('actions.newCollection')}
          </Button>
        </div>
      </div>

      {/* Stats Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {t('stats.totalFiles')}
            </CardTitle>
            <Image className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">2,847</div>
            <p className="text-xs text-muted-foreground">
              +180 from last month
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {t('stats.totalSize')}
            </CardTitle>
            <Archive className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">47.2 GB</div>
            <p className="text-xs text-muted-foreground">
              +2.1 GB from last month
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {t('stats.categories')}
            </CardTitle>
            <FolderOpen className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">24</div>
            <p className="text-xs text-muted-foreground">
              +2 new categories
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {t('stats.collections')}
            </CardTitle>
            <Archive className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">156</div>
            <p className="text-xs text-muted-foreground">
              +12 new collections
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Main Content Tabs */}
      <Card>
        <CardHeader>
          <CardTitle>{t('content.title')}</CardTitle>
        </CardHeader>
        <CardContent>
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <TabsList className="grid w-full grid-cols-6">
              {tabs.map((tab) => (
                <TabsTrigger 
                  key={tab.id} 
                  value={tab.id}
                  className="flex items-center space-x-2"
                >
                  {tab.icon}
                  <span className="hidden sm:inline">{tab.label}</span>
                </TabsTrigger>
              ))}
            </TabsList>

            {tabs.map((tab) => (
              <TabsContent key={tab.id} value={tab.id} className="mt-6">
                {tab.component}
              </TabsContent>
            ))}
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
};
