import React from 'react';
import { useTranslation } from 'react-i18next';
import { FolderOpen, Plus, Edit3, Trash2 } from 'lucide-react';
import { Button, Card, CardContent, CardHeader, CardTitle } from 'mada-design-system';

export const MediaCategories: React.FC = () => {
  const { t } = useTranslation('media-management');

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
            {t('categories.title')}
          </h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            {t('categories.description')}
          </p>
        </div>
        <Button>
          <Plus className="w-4 h-4 mr-2" />
          {t('categories.newCategory')}
        </Button>
      </div>

      {/* Categories Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-blue-100 dark:bg-blue-900/20 rounded-lg flex items-center justify-center">
                  <FolderOpen className="w-5 h-5 text-blue-600" />
                </div>
                <div>
                  <CardTitle className="text-lg">Images</CardTitle>
                  <p className="text-sm text-gray-500">Photo and image files</p>
                </div>
              </div>
              <div className="flex items-center space-x-1">
                <Button variant="ghost" size="sm">
                  <Edit3 className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="sm">
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-between text-sm">
              <span className="text-gray-600">156 files</span>
              <span className="text-gray-400">Created 2 days ago</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-purple-100 dark:bg-purple-900/20 rounded-lg flex items-center justify-center">
                  <FolderOpen className="w-5 h-5 text-purple-600" />
                </div>
                <div>
                  <CardTitle className="text-lg">Videos</CardTitle>
                  <p className="text-sm text-gray-500">Video and movie files</p>
                </div>
              </div>
              <div className="flex items-center space-x-1">
                <Button variant="ghost" size="sm">
                  <Edit3 className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="sm">
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-between text-sm">
              <span className="text-gray-600">89 files</span>
              <span className="text-gray-400">Created 1 week ago</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-green-100 dark:bg-green-900/20 rounded-lg flex items-center justify-center">
                  <FolderOpen className="w-5 h-5 text-green-600" />
                </div>
                <div>
                  <CardTitle className="text-lg">Documents</CardTitle>
                  <p className="text-sm text-gray-500">PDF and document files</p>
                </div>
              </div>
              <div className="flex items-center space-x-1">
                <Button variant="ghost" size="sm">
                  <Edit3 className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="sm">
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-between text-sm">
              <span className="text-gray-600">234 files</span>
              <span className="text-gray-400">Created 3 days ago</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};
