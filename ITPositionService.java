/*
 * Copyright 2013 Primeton Technologies Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.gocom.components.nui.demo;

import com.eos.das.entity.criteria.CriteriaType;
import org.gocom.components.nui.demo.newdataset.TPosition;

/**
 * 完成员工职位的增删改查功能
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public interface ITPositionService{

	/**
	 *
	 * @param tPosition TPosition
	 */
	void addTPosition(TPosition tPosition);

	/**
	 *
	 * @param tPositions TPosition[]
	 */
	void deleteTPosition(TPosition[] tPositions);

	/**
	 *
	 * @param tPosition TPosition[]
	 */
	void getTPosition(TPosition tPosition);

	/**
	 *
	 * @param criteria CriteriaType
	 * @param page PageCond
	 * @return TPosition[]
	 */
	TPosition[] queryTPositions(CriteriaType criteriaType);

	/**
	 *
	 * @param tPosition TPosition[]
	 */
	void updateTPosition(TPosition tPosition);
	
}

