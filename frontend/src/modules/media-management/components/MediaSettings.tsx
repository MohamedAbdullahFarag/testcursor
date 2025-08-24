import React from 'react';
import { useTranslation } from 'react-i18next';
import { Settings, Save, HardDrive, Image, Shield, Bell, Database } from 'lucide-react';
import { Button, Card, CardContent, CardHeader, CardTitle, Input, Label, Switch, Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from 'mada-design-system';

export const MediaSettings: React.FC = () => {
  const { t } = useTranslation('media-management');

  return (
    <div className="space-y-6">
      <div className="text-center">
        <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
          {t('settings.title')}
        </h2>
        <p className="text-gray-600 dark:text-gray-400 mt-2">
          {t('settings.description')}
        </p>
      </div>

      {/* Storage Settings */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <HardDrive className="w-5 h-5" />
            <span>{t('settings.storage')}</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="maxFileSize">{t('settings.maxFileSize')}</Label>
              <Select defaultValue="100">
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="10">10 MB</SelectItem>
                  <SelectItem value="50">50 MB</SelectItem>
                  <SelectItem value="100">100 MB</SelectItem>
                  <SelectItem value="500">500 MB</SelectItem>
                  <SelectItem value="1000">1 GB</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="storagePath">Storage Path</Label>
              <Input id="storagePath" defaultValue="/media" />
            </div>
          </div>
          <div>
            <Label htmlFor="allowedTypes">{t('settings.allowedTypes')}</Label>
            <Input id="allowedTypes" defaultValue="jpg,jpeg,png,gif,pdf,doc,docx,mp4,avi,mov,mp3,wav" />
          </div>
        </CardContent>
      </Card>

      {/* Processing Settings */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Image className="w-5 h-5" />
            <span>{t('settings.processing')}</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="autoThumbnails">{t('settings.autoThumbnails')}</Label>
              <p className="text-sm text-gray-500">Automatically generate thumbnails for images</p>
            </div>
            <Switch id="autoThumbnails" defaultChecked />
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="thumbnailQuality">Thumbnail Quality</Label>
              <Select defaultValue="85">
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="60">60%</SelectItem>
                  <SelectItem value="75">75%</SelectItem>
                  <SelectItem value="85">85%</SelectItem>
                  <SelectItem value="95">95%</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="maxThumbnailSize">Max Thumbnail Size</Label>
              <Select defaultValue="1200">
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="800">800px</SelectItem>
                  <SelectItem value="1200">1200px</SelectItem>
                  <SelectItem value="1920">1920px</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Security Settings */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Shield className="w-5 h-5" />
            <span>{t('settings.security')}</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="virusScan">Virus Scanning</Label>
              <p className="text-sm text-gray-500">Scan uploaded files for viruses</p>
            </div>
            <Switch id="virusScan" defaultChecked />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="fileValidation">File Validation</Label>
              <p className="text-sm text-gray-500">Validate file headers and content</p>
            </div>
            <Switch id="fileValidation" defaultChecked />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="accessLogging">Access Logging</Label>
              <p className="text-sm text-gray-500">Log all file access attempts</p>
            </div>
            <Switch id="accessLogging" defaultChecked />
          </div>
        </CardContent>
      </Card>

      {/* Notification Settings */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Bell className="w-5 h-5" />
            <span>{t('settings.notifications')}</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="uploadNotifications">Upload Notifications</Label>
              <p className="text-sm text-gray-500">Notify when files are uploaded</p>
            </div>
            <Switch id="uploadNotifications" defaultChecked />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="processingNotifications">Processing Notifications</Label>
              <p className="text-sm text-gray-500">Notify when processing is complete</p>
            </div>
            <Switch id="processingNotifications" defaultChecked />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <Label htmlFor="errorNotifications">Error Notifications</Label>
              <p className="text-sm text-gray-500">Notify when errors occur</p>
            </div>
            <Switch id="errorNotifications" defaultChecked />
          </div>
        </CardContent>
      </Card>

      {/* Database Settings */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Database className="w-5 h-5" />
            <span>Database</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="backupFrequency">Backup Frequency</Label>
              <Select defaultValue="daily">
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="hourly">Hourly</SelectItem>
                  <SelectItem value="daily">Daily</SelectItem>
                  <SelectItem value="weekly">Weekly</SelectItem>
                  <SelectItem value="monthly">Monthly</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="retentionPeriod">Retention Period</Label>
              <Select defaultValue="30">
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="7">7 days</SelectItem>
                  <SelectItem value="30">30 days</SelectItem>
                  <SelectItem value="90">90 days</SelectItem>
                  <SelectItem value="365">1 year</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Save Button */}
      <div className="flex justify-end">
        <Button className="w-full md:w-auto">
          <Save className="w-4 h-4 mr-2" />
          Save Settings
        </Button>
      </div>
    </div>
  );
};
