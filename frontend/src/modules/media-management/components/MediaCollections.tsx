import React from 'react';
import { useTranslation } from 'react-i18next';
import { Archive, Plus, Edit3, Trash2, Eye, Users } from 'lucide-react';
import { Button, Card, CardContent, CardHeader, CardTitle, Badge } from 'mada-design-system';

export const MediaCollections: React.FC = () => {
  const { t } = useTranslation('media-management');

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
            {t('collections.title')}
          </h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            {t('collections.description')}
          </p>
        </div>
        <Button>
          <Plus className="w-4 h-4 mr-2" />
          {t('collections.newCollection')}
        </Button>
      </div>

      {/* Collections Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-blue-100 dark:bg-blue-900/20 rounded-lg flex items-center justify-center">
                  <Archive className="w-5 h-5 text-blue-600" />
                </div>
                <div>
                  <CardTitle className="text-lg">Summer Vacation 2024</CardTitle>
                  <p className="text-sm text-gray-500">Photos from our summer trip</p>
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
            <div className="space-y-3">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">24 items</span>
                <div className="flex items-center space-x-2">
                  <Badge variant="secondary">Album</Badge>
                  <Badge variant="outline">Public</Badge>
                </div>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-400">Created 1 week ago</span>
                <div className="flex items-center space-x-1 text-gray-500">
                  <Eye className="w-4 h-4" />
                  <span>156 views</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-purple-100 dark:bg-purple-900/20 rounded-lg flex items-center justify-center">
                  <Archive className="w-5 h-5 text-purple-600" />
                </div>
                <div>
                  <CardTitle className="text-lg">Product Gallery</CardTitle>
                  <p className="text-sm text-gray-500">Company product images</p>
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
            <div className="space-y-3">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">67 items</span>
                <div className="flex items-center space-x-2">
                  <Badge variant="secondary">Gallery</Badge>
                  <Badge variant="outline">Public</Badge>
                </div>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-400">Created 2 weeks ago</span>
                <div className="flex items-center space-x-1 text-gray-500">
                  <Eye className="w-4 h-4" />
                  <span>89 views</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-green-100 dark:bg-green-900/20 rounded-lg flex items-center justify-center">
                  <Archive className="w-5 h-5 text-green-600" />
                </div>
                <div>
                  <CardTitle className="text-lg">Portfolio</CardTitle>
                  <p className="text-sm text-gray-500">Professional work samples</p>
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
            <div className="space-y-3">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">45 items</span>
                <div className="flex items-center space-x-2">
                  <Badge variant="secondary">Portfolio</Badge>
                  <Badge variant="outline">Featured</Badge>
                </div>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-400">Created 1 month ago</span>
                <div className="flex items-center space-x-1 text-gray-500">
                  <Eye className="w-4 h-4" />
                  <span>234 views</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};
