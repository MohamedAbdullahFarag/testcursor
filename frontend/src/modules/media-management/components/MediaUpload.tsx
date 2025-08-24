import React from 'react';
import { useTranslation } from 'react-i18next';
import { Upload, File, X } from 'lucide-react';
import { Button, Card, CardContent, CardHeader, CardTitle } from 'mada-design-system';

export const MediaUpload: React.FC = () => {
  const { t } = useTranslation('media-management');

  return (
    <div className="space-y-6">
      <div className="text-center">
        <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
          {t('upload.title')}
        </h2>
        <p className="text-gray-600 dark:text-gray-400 mt-2">
          {t('upload.description')}
        </p>
      </div>

      {/* Drag & Drop Zone */}
      <Card className="border-2 border-dashed border-gray-300 dark:border-gray-600 hover:border-gray-400 dark:hover:border-gray-500 transition-colors">
        <CardContent className="p-12 text-center">
          <Upload className="mx-auto h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
            {t('upload.dragDropText')}
          </h3>
          <p className="text-sm text-gray-500 mb-4">
            {t('upload.supportedFormats')}
          </p>
          <p className="text-xs text-gray-400 mb-6">
            {t('upload.maxFileSize')}
          </p>
          <Button>
            <File className="w-4 h-4 mr-2" />
            Choose Files
          </Button>
        </CardContent>
      </Card>

      {/* Upload Progress */}
      <Card>
        <CardHeader>
          <CardTitle>Upload Progress</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-800 rounded-lg">
              <div className="flex items-center space-x-3">
                <File className="w-5 h-5 text-blue-500" />
                <div>
                  <p className="font-medium">sample-image.jpg</p>
                  <p className="text-sm text-gray-500">2.4 MB</p>
                </div>
              </div>
              <div className="flex items-center space-x-2">
                <div className="w-32 bg-gray-200 rounded-full h-2">
                  <div className="bg-blue-600 h-2 rounded-full" style={{ width: '75%' }}></div>
                </div>
                <span className="text-sm text-gray-600">75%</span>
                <Button variant="ghost" size="sm">
                  <X className="w-4 h-4" />
                </Button>
              </div>
            </div>

            <div className="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-800 rounded-lg">
              <div className="flex items-center space-x-3">
                <File className="w-5 h-5 text-green-500" />
                <div>
                  <p className="font-medium">document.pdf</p>
                  <p className="text-sm text-gray-500">1.8 MB</p>
                </div>
              </div>
              <div className="flex items-center space-x-2">
                <span className="text-sm text-green-600">✓ Complete</span>
                <Button variant="ghost" size="sm">
                  <X className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Upload Settings */}
      <Card>
        <CardHeader>
          <CardTitle>Upload Settings</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Default Category
              </label>
              <select className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
                <option>Select Category</option>
                <option>Images</option>
                <option>Documents</option>
                <option>Videos</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Privacy
              </label>
              <select className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
                <option>Public</option>
                <option>Private</option>
                <option>Shared</option>
              </select>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
